using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Application.Commands.AddExpensesBatch;
using ExpenseTracker.Application.Commands.DeleteExpense;
using ExpenseTracker.Application.Commands.ExtractReceiptExpenses;
using ExpenseTracker.Application.Commands.UpdateExpense;
using ExpenseTracker.Application.Queries.GetAllExpenses;
using ExpenseTracker.Application.Queries.GetExpenseById;
using ExpenseTracker.Application.Queries.GetExpensesByReceiptId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/expensetable/{tableId}/expenses")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        public IMediator _mediator { get; }

        public ExpenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/expensetable/{tableId}/expenses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int tableId, int id, CancellationToken cancellationToken)
        {
           var expense = await _mediator.Send(new GetExpenseByIdQuery { Id = id }, cancellationToken);
           return Ok(expense);
        }

        // GET api/expensetable/{tableId}/expenses
        [HttpGet]
        public async Task<IActionResult> GetAll(int tableId, CancellationToken cancellationToken)
        {
            var expenses = await _mediator.Send(new GetAllExpensesQuery { ExpenseTableId = tableId }, cancellationToken);
            return Ok(expenses);
        }

        // GET api/expensetable/{tableId}/expenses/by-receipt/5
        [HttpGet("by-receipt/{receiptId}")]
        public async Task<IActionResult> GetByReceipt(int tableId, int receiptId, CancellationToken cancellationToken)
        {
            var expenses = await _mediator.Send(new GetExpensesByReceiptIdQuery { ReceiptId = receiptId, ExpenseTableId = tableId }, cancellationToken);
            return Ok(expenses);
        }

        // POST api/expensetable/{tableId}/expenses
        [HttpPost]
        public async Task<IActionResult> AddExpense(int tableId, [FromBody] AddExpenseCommand command, CancellationToken cancellationToken)
        {
            if (tableId != command.ExpenseTableId)
                return BadRequest("Route tableId and body ExpenseTableId must match.");

            int id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { tableId, id }, new { id });
        }

        // POST api/expensetable/{tableId}/expenses/batch
        [HttpPost("batch")]
        public async Task<IActionResult> AddExpensesBatch(int tableId, [FromBody] List<AddExpenseCommand> items, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AddExpensesBatchCommand(tableId, items), cancellationToken);
            return Ok(result);
        }

        private static readonly string[] AllowedReceiptContentTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxReceiptFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        // POST api/expensetable/{tableId}/expenses/extract-receipt
        [HttpPost("extract-receipt")]
        [RequestSizeLimit(MaxReceiptFileSizeBytes)]
        public async Task<IActionResult> ExtractReceipt(int tableId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                return BadRequest("A receipt image file is required.");

            if (file.Length > MaxReceiptFileSizeBytes)
                return BadRequest("Image is too large. Maximum size is 10 MB.");

            if (!AllowedReceiptContentTypes.Contains(file.ContentType))
                return BadRequest("Unsupported image type. Please upload a JPEG, PNG, or WEBP image.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);

            var result = await _mediator.Send(new ExtractReceiptExpensesCommand(stream.ToArray(), file.ContentType), cancellationToken);
            return Ok(result);
        }

        // PUT api/expensetable/{tableId}/expenses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int tableId, int id, [FromBody] UpdateExpenseCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("Route id and body id must match.");

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // DELETE api/expensetable/{tableId}/expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int tableId, int id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteExpenseCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
