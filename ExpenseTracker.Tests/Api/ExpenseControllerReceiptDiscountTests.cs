using ExpenseTracker.Application.Commands.UpdateReceiptDiscount;
using ExpenseTrackerAPI.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExpenseTracker.Tests.Api
{
    public class ExpenseControllerReceiptDiscountTests
    {
        [Fact]
        public async Task UpdateReceiptDiscount_SendsTheCommandWithTheRouteIdsAndReturnsNoContent()
        {
            var mediator = new Mock<IMediator>();
            var controller = new ExpenseController(mediator.Object);

            // The body only carries the discount; any ids in it must be replaced by the route's.
            var result = await controller.UpdateReceiptDiscount(7, 9, new UpdateReceiptDiscountCommand(2.50m, 99, 98), CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();
            mediator.Verify(m => m.Send(
                It.Is<UpdateReceiptDiscountCommand>(c => c.ReceiptDiscount == 2.50m && c.ExpenseTableId == 7 && c.ReceiptId == 9),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
