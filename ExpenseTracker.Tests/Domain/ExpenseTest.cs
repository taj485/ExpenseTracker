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
            expense.UnitPrice.Amount.Should().Be(50m);
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
        public void Create_WithoutQuantity_DefaultsToOne()
        {
            // Arrange & Act
            var expense = Expense.Create(50m, ExpenseCategory.Food, "Lunch", DateTime.UtcNow, TestTableId);
            // Assert
            expense.Quantity.Should().Be(1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Create_WithQuantityBelowOne_ThrowsDomainException(int quantity)
        {
            // Arrange & Act
            Action act = () => Expense.Create(50m, ExpenseCategory.Food, "Lunch", DateTime.UtcNow, TestTableId, quantity: quantity);
            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Quantity must be at least 1");
        }

        [Fact]
        public void UpdateUnitPrice_WithValidUnitPrice_UpdatesUnitPrice()
        {
            // Arrange
            var expense = Expense.Create(30m, ExpenseCategory.Utilities, "Electricity bill", DateTime.UtcNow, TestTableId);
            var newUnitPrice = 35m;
            // Act
            expense.UpdateUnitPrice(newUnitPrice);
            // Assert
            expense.UnitPrice.Amount.Should().Be(35m);
        }

        [Fact]
        public void UpdateQuantity_WithValidQuantity_UpdatesQuantity()
        {
            // Arrange
            var expense = Expense.Create(30m, ExpenseCategory.Utilities, "Electricity bill", DateTime.UtcNow, TestTableId);
            // Act
            expense.UpdateQuantity(5);
            // Assert
            expense.Quantity.Should().Be(5);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void UpdateQuantity_WithQuantityBelowOne_ThrowsDomainException(int quantity)
        {
            // Arrange
            var expense = Expense.Create(30m, ExpenseCategory.Utilities, "Electricity bill", DateTime.UtcNow, TestTableId);
            // Act
            Action act = () => expense.UpdateQuantity(quantity);
            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Quantity must be at least 1");
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

        [Fact]
        public void Create_WithDiscountEqualToLineTotal_IsAllowed()
        {
            var expense = Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TestTableId, quantity: 2, discount: 3m);

            expense.Discount.Should().Be(3m);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(3.01)]
        public void Create_WithDiscountOutsideLineTotal_Throws(double discount)
        {
            Action act = () => Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TestTableId, quantity: 2, discount: (decimal)discount);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Create_WithNegativeDiscount_ExplainsWhy()
        {
            Action act = () => Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TestTableId, discount: -1m);

            act.Should().Throw<DomainException>().WithMessage("Discount can't be negative.");
        }

        [Fact]
        public void Create_WithDiscountAboveLineTotal_ExplainsWhy()
        {
            Action act = () => Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TestTableId, discount: 2m);

            act.Should().Throw<DomainException>().WithMessage("Discount can't be more than the item total.");
        }
    
        [Fact]
        public void UpdateDiscount_SetsAndClearsTheDiscount()
        {
            var expense = Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TestTableId, quantity: 2, discount: 0.50m);

            expense.UpdateDiscount(3.00m);
            expense.Discount.Should().Be(3.00m);

            expense.UpdateDiscount(0m);
            expense.Discount.Should().Be(0m);
        }

        [Fact]
        public void UpdateDiscount_WithNegativeDiscount_Throws()
        {
            var expense = Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TestTableId, quantity: 2);

            Action act = () => expense.UpdateDiscount(-0.50m);

            act.Should().Throw<DomainException>().WithMessage("Discount can't be negative.");
        }

        [Fact]
        public void UpdateDiscount_AboveTheLineTotal_ThrowsWithTheTotal()
        {
            var expense = Expense.Create(1.50m, ExpenseCategory.Food, "Milk", DateTime.UtcNow, TestTableId, quantity: 2);

            Action act = () => expense.UpdateDiscount(3.01m);

            act.Should().Throw<DomainException>().WithMessage("Discount can't be more than the item total (£3.00).");
        }
    }
}
