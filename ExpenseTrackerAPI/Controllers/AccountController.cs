using ExpenseTracker.Application.Commands.DeleteAccount;
using ExpenseTracker.Application.Commands.GiveAiReceiptConsent;
using ExpenseTracker.Application.Commands.WithdrawAiReceiptConsent;
using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Queries.GetAiReceiptConsent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // DELETE api/account
        [HttpDelete]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAccountCommand(), cancellationToken);
            return NoContent();
        }

        // GET api/account/ai-receipt-consent
        [HttpGet("ai-receipt-consent")]
        public async Task<ActionResult<AiReceiptConsentDto>> GetAiReceiptConsent(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAiReceiptConsentQuery(), cancellationToken));
        }

        // PUT api/account/ai-receipt-consent
        [HttpPut("ai-receipt-consent")]
        public async Task<IActionResult> GiveAiReceiptConsent([FromBody] GiveAiReceiptConsentCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // DELETE api/account/ai-receipt-consent
        [HttpDelete("ai-receipt-consent")]
        public async Task<IActionResult> WithdrawAiReceiptConsent(CancellationToken cancellationToken)
        {
            await _mediator.Send(new WithdrawAiReceiptConsentCommand(), cancellationToken);
            return NoContent();
        }
    }
}
