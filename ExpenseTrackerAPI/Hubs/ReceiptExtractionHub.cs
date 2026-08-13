using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Queries.GetReceiptExtractionStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseTrackerAPI.Hubs
{
    [Authorize]
    public class ReceiptExtractionHub : Hub
    {
        public const string CompletedMethod = "ExtractionCompleted";

        private readonly IMediator _mediator;

        public ReceiptExtractionHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public static string GroupFor(Guid jobId) => $"extraction-{jobId}";

        /// <summary>
        /// Joins the caller to a job's push group and hands back the current status.
        /// </summary>
        /// <remarks>
        /// Returning the status is what closes the obvious race: extraction can finish before the
        /// client gets here, in which case the completion push has already fired into an empty group
        /// and no further message is ever coming. The query also authorizes — it 404s unless the job
        /// belongs to the caller — so the ownership check happens before the group join, not after.
        /// </remarks>
        public async Task<ReceiptExtractionStatusDto> Subscribe(Guid jobId)
        {
            var status = await _mediator.Send(new GetReceiptExtractionStatusQuery(jobId));

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupFor(jobId));

            return status;
        }

        public async Task Unsubscribe(Guid jobId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupFor(jobId));
        }
    }
}
