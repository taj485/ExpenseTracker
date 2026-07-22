using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;
using System;

namespace ExpenseTracker.Tests.Domain
{
    public class ExpenseTest
    {
        private const int TestTableId = 1;

        [Fact]
        public void Create_WithValidData_ReturnsExpense()
        {
            // Arrange
            var amount = 50m;
            var category = ExpenseCategory.Food;
            var description = "Lunch";

            // Act
            var expense = Expense.Create(amount, category, description, DateTime.UtcNow, TestTableId);

            // Assert
            expense.Amount.Amount.Should().Be(50m);
            expense.Category.Should().Be(ExpenseCategory.Food);
            expense.Description.Should().Be("Lunch");
            expense.Date.Date.Should().Be(DateTime.UtcNow.Date);
            expense.IsDeleted.Should().BeFalse();
            expense.ExpenseTableId.Should().Be(TestTableId);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(0)]
        public void Create_WithNegativeAmount_ThrowsDomainException(decimal amount)
        {
            // Arrange
            var category = ExpenseCategory.Entertainment;
            var description = "Movie";
            // Act
            Action act = () => Expense.Create(amount, category, description, DateTime.UtcNow, TestTableId);
            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Amount must be greater than zero.");
        }

        [Fact]
        public void Create_WithEmptyDescription_ThrowsDomainException()
        {
            // Arrange
            var amount = 20m;
            var category = ExpenseCategory.Transport;
            var description = "";
            // Act
            Action act = () => Expense.Create(amount, category, description, DateTime.UtcNow, TestTableId);
            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Description is required");
        }

        [Fact]
        public void UpdateAmount_WithValidAmount_UpdatesAmount()
        {
            // Arrange
            var expense = Expense.Create(30m, ExpenseCategory.Utilities, "Electricity bill", DateTime.UtcNow, TestTableId);
            var newAmount = 35m;
            // Act
            expense.UpdateAmount(newAmount);
            // Assert
            expense.Amount.Amount.Should().Be(35m);
        }

        [Fact]
        public void UpdateDescription_WithValidDescription_UpdatesDescription()
        {
            // Arrange
            var expense = Expense.Create(15m, ExpenseCategory.Health, "Doctor visit", DateTime.UtcNow, TestTableId);
            var newDescription = "Dentist visit";
            // Act
            expense.UpdateDescription(newDescription);
            // Assert
            expense.Description.Should().Be("Dentist visit");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        public void UpdateDescription_WithEmptyDescription_ThrowsDomainException(string newDescription)
        {
            // Arrange
            var expense = Expense.Create(15m, ExpenseCategory.Health, "Doctor visit", DateTime.UtcNow, TestTableId);
            // Act
            Action act = () => expense.UpdateDescription(newDescription);
            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Description cannot be empty");
        }

        [Fact]
        public void UpdateCategory_WithValidCategory_UpdatesCategory()
        {
            // Arrange
            var expense = Expense.Create(15m, ExpenseCategory.Health, "Doctor visit", DateTime.UtcNow, TestTableId);
            // Act
            expense.UpdateCategory(ExpenseCategory.Transport);
            // Assert
            expense.Category.Should().Be(ExpenseCategory.Transport);
        }
    }
}
