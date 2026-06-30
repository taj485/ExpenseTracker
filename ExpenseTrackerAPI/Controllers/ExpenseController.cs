using ExpenseTracker.Application.Commands.AddExpense;
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
        public async Task<IActionResult> Get(int id)
        {
           var expense = await _mediator.Send(new GetExpenseByIdQuery { Id = id });
           return Ok(expense);
        }

        // POST api/<ExpenseTracker>
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] AddExpenseCommand command)
        {
            int id = await _mediator.Send(command);
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
