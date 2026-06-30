using ExpenseTracker.Application.Queries.GetExpenseById;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Tests.Application.Queries
{
    public class GetExpenseQueryHandlerTests
    {
        private readonly Mock<IExpenseReader> _mockReader;
        private readonly GetExpenseByIdQueryHandler _handler;

        public GetExpenseQueryHandlerTests()
        {
            _mockReader = new Mock<IExpenseReader>();
            _handler = new GetExpenseByIdQueryHandler(_mockReader.Object);
        }


        [Fact]
        public async Task Handle_ReturnsDto_WhenExpenseExist()
        {
            //Arrange
            var expense = Expense.Create(1, ExpenseCategory.Transport, "Bus Fare");
            _mockReader.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);

            //Act
            var result = await _handler.Handle(new GetExpenseByIdQuery { Id = 1 }, CancellationToken.None);

            //Assert
            Assert.Equal("Bus Fare", result.Description);
        }

        [Fact]
        public async Task Handle_ThrowsValidationException_WhenIdIsZeroOrLess()
        {
            //Arrange
            var query = new GetExpenseByIdQuery { Id = 0 };

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(query, CancellationToken.None));

            //Assert
            Assert.Contains(exception.Errors, e => e.PropertyName == "Id");
        }
    }
}
