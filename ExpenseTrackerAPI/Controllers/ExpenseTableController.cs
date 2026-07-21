using ExpenseTracker.Application.Commands.CreateExpenseTable;
using ExpenseTracker.Application.Commands.DeleteExpenseTable;
using ExpenseTracker.Application.Commands.InviteUserToTable;
using ExpenseTracker.Application.Commands.RemoveUserFromTable;
using ExpenseTracker.Application.Queries.GetExpenseTablesForUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseTableController : ControllerBase
    {
        public IMediator _mediator { get; }

        public ExpenseTableController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/expensetable
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var tables = await _mediator.Send(new GetExpenseTablesForUserQuery(), cancellationToken);
            return Ok(tables);
        }

        // POST api/expensetable
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseTableCommand command, CancellationToken cancellationToken)
        {
            int id = await _mediator.Send(command, cancellationToken);
            return Ok(new { id });
        }

        // POST api/expensetable/5/members
        [HttpPost("{id}/members")]
        public async Task<IActionResult> InviteMember(int id, [FromBody] InviteUserToTableCommand command, CancellationToken cancellationToken)
        {
            if (id != command.ExpenseTableId)
                return BadRequest("Route id and body ExpenseTableId must match.");

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // DELETE api/expensetable/5/members/7
        [HttpDelete("{id}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int id, int userId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoveUserFromTableCommand(id, userId), cancellationToken);
            return NoContent();
        }

        // DELETE api/expensetable/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteExpenseTableCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
