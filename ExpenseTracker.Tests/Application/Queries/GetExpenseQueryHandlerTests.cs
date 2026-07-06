using ExpenseTracker.Application.Queries.GetExpenseById;
using ExpenseTracker.Application.Services;
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
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly GetExpenseByIdQueryHandler _handler;

        public GetExpenseQueryHandlerTests()
        {
            _mockReader = new Mock<IExpenseReader>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            _handler = new GetExpenseByIdQueryHandler(_mockReader.Object, _mockCurrentUserProvider.Object);
        }


        [Fact]
        public async Task Handle_ReturnsDto_WhenExpenseExist()
        {
            //Arrange
            var expense = Expense.Create(1, ExpenseCategory.Transport, "Bus Fare", _currentUser);
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

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenCurrentUserNotAssociated()
        {
            //Arrange
            var otherUser = User.Create("auth0|other-user");
            otherUser.Id = 2;
            var expense = Expense.Create(1, ExpenseCategory.Transport, "Bus Fare", otherUser);
            _mockReader.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expense);

            //Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new GetExpenseByIdQuery { Id = 1 }, CancellationToken.None));
        }
    }
}