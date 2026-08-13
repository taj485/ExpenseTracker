import { Injectable, inject } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ReceiptExtractionStatus } from '../models/expense.model';

const COMPLETED_EVENT = 'ExtractionCompleted';

/** How long to wait for a push before giving up on the socket and letting the caller poll. */
const PUSH_TIMEOUT_MS = 10_000;

function isTerminal(status: ReceiptExtractionStatus): boolean {
  return status.status === 'Completed' || status.status === 'Failed';
}

/**
 * Wraps the receipt-extraction hub. Returns null whenever the realtime path can't deliver, which is
 * the caller's signal to fall back to polling — a failed socket must never be a failed extraction.
 */
@Injectable({ providedIn: 'root' })
export class ReceiptExtractionRealtimeService {
  private readonly auth = inject(AuthService);
  private connection: HubConnection | null = null;
  private starting: Promise<HubConnection | null> | null = null;

  /**
   * Subscribes to a job and resolves with its terminal status, or null if realtime is unavailable
   * or nothing arrived in time.
   */
  async awaitResult(jobId: string): Promise<ReceiptExtractionStatus | null> {
    const connection = await this.ensureConnected();
    if (!connection) return null;

    let onCompleted: ((status: ReceiptExtractionStatus) => void) | null = null;

    try {
      // Register before subscribing so a push can't slip through the gap between the two.
      const pushed = new Promise<ReceiptExtractionStatus | null>((resolve) => {
        onCompleted = (status: ReceiptExtractionStatus) => {
          if (status?.jobId === jobId) resolve(status);
        };
        connection.on(COMPLETED_EVENT, onCompleted);
        setTimeout(() => resolve(null), PUSH_TIMEOUT_MS);
      });

      // Subscribe returns the current status, which closes the race where extraction finished
      // before we got here — that push already fired into an empty group and is never coming back.
      const current = await connection.invoke<ReceiptExtractionStatus>('Subscribe', jobId);
      if (current && isTerminal(current)) return current;

      return await pushed;
    } catch {
      // Hub rejected us (commonly a 401 when the token never reached the handshake). Poll instead.
      return null;
    } finally {
      if (onCompleted) connection.off(COMPLETED_EVENT, onCompleted);
      void connection.invoke('Unsubscribe', jobId).catch(() => {});
    }
  }

  private ensureConnected(): Promise<HubConnection | null> {
    if (this.connection?.state === HubConnectionState.Connected) {
      return Promise.resolve(this.connection);
    }

    // Collapse concurrent uploads onto a single in-flight start.
    this.starting ??= this.start().finally(() => {
      this.starting = null;
    });

    return this.starting;
  }

  private async start(): Promise<HubConnection | null> {
    try {
      const connection = new HubConnectionBuilder()
        .withUrl(environment.receiptExtractionHubUrl, {
          // A browser can't set an Authorization header on a WebSocket handshake, so signalr
          // appends this as ?access_token=. The API reads it back for /hubs paths.
          accessTokenFactory: () => firstValueFrom(this.auth.getAccessTokenSilently()),
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Warning)
        .build();

      await connection.start();
      this.connection = connection;
      return connection;
    } catch {
      this.connection = null;
      return null;
    }
  }
}
