using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Application.Commands.DeleteExpense;
using ExpenseTracker.Application.Commands.UpdateExpense;
using ExpenseTracker.Application.Queries.GetAllExpenses;
using ExpenseTracker.Application.Queries.GetExpenseById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        public IMediator _mediator { get; }

        public ExpenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/<ExpenseTracker>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
           var expense = await _mediator.Send(new GetExpenseByIdQuery { Id = id }, cancellationToken);
           return Ok(expense);
        }

        // GET api/<ExpenseTracker>/
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var expenses = await _mediator.Send(new GetAllExpensesQuery(), cancellationToken);
            return Ok(expenses);
        }

        // POST api/<ExpenseTracker>
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] AddExpenseCommand command, CancellationToken cancellationToken)
        {
            int id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }

        // PUT api/<ExpenseTracker>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("Route id and body id must match.");

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // DELETE api/<ExpenseTracker>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteExpenseCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
