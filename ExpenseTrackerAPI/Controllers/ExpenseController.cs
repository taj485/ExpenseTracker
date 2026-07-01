using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Application.Queries.GetAllExpenses;
using ExpenseTracker.Application.Queries.GetExpenseById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExpenseTracker>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
