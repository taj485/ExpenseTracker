# Client (Angular 22)

Standalone components, signals, `inject()`, Vitest via `ng test`. Design tokens live in `src/styles.css`.

## Where things go
- `src/app/features/<feature>/`: routed screens and their components (`*.component.ts/.html/.css`, `ChangeDetectionStrategy.OnPush`).
- `src/app/core/services/`: one `@Injectable({ providedIn: 'root' })` service per API area, with state held in signals.
- `src/app/core/models/`: API types. `src/app/core/utils/`: pure functions, each with a `.spec.ts` beside it.
- `src/app/shared/`: reusable components and directives. `src/app/layout/`: shell, sidebar, topbar.

## Patterns to copy
- API service: `core/services/expense-table.service.ts`. It uses `inject(HttpClient)` with a URL built from `environment.apiUrl`, puts a `// API CALL: METHOD /api/...` comment above each method, and turns errors into messages with `core/utils/api-error.utils.ts`.
- Component: `features/upload-receipt/upload-receipt.component.ts`. State lives in signals and `computed`.
- Tests: Vitest (`vi.fn()`) + `TestBed`, mocking `AuthService` with `useValue` (see `core/auth/auth.guard.spec.ts`).

## CSS
- Never use `nth-child` selectors to target columns/elements. Add a human-readable class name instead (e.g. `.col-date` rather than `td:nth-child(1)`), so styles stay readable and don't break silently if ordering changes.
- Each component's styles have a size budget of 4 kB (warning) and 8 kB (error), set in `angular.json`. Put shared styles in `src/styles.css` or a shared CSS file rather than growing one component.

## Commands (from `Client/`)
Always run the CLI non-interactively; otherwise it can hang on a hidden prompt:
- Build: `NG_CLI_ANALYTICS=false CI=1 npx ng build < /dev/null`
- Test: `NG_CLI_ANALYTICS=false CI=1 npx ng test --watch=false < /dev/null 2>&1 | grep -E "Test Files|Tests |FAIL|Error"`. If it prints "Worker exited unexpectedly" before any tests run, run it again once.
- Run: `npm run start:ai` (AI dev mode; needs the API running with `--launch-profile ai`). `/api` is proxied to `localhost:5252`.
