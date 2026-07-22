using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;

namespace ExpenseTracker.Tests.Domain
{
    public class ExpenseTableTests
    {
        [Fact]
        public void Create_WithValidName_ReturnsTableWithCreatorAsAdmin()
        {
            // Act
            var table = ExpenseTable.Create("Household", 1);

            // Assert
            table.Name.Should().Be("Household");
            table.CreatedByUserId.Should().Be(1);
            table.IsDeleted.Should().BeFalse();
            table.Members.Should().ContainSingle();
            table.Members.Single().UserId.Should().Be(1);
            table.Members.Single().IsAdmin.Should().BeTrue();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        public void Create_WithEmptyName_ThrowsDomainException(string name)
        {
            // Act
            Action act = () => ExpenseTable.Create(name, 1);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Name is required");
        }

        [Fact]
        public void AddMember_WithNewUser_AddsToMembers()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);

            // Act
            table.AddMember(2, isAdmin: false);

            // Assert
            table.Members.Should().HaveCount(2);
            table.Members.Should().Contain(m => m.UserId == 2 && !m.IsAdmin);
        }

        [Fact]
        public void AddMember_WithDuplicateUser_ThrowsDomainException()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);

            // Act
            Action act = () => table.AddMember(1, isAdmin: false);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("User is already a member of this expense table");
        }

        [Fact]
        public void RemoveMember_WithExistingNonAdminMember_RemovesThem()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);
            table.AddMember(2, isAdmin: false);

            // Act
            table.RemoveMember(2);

            // Assert
            table.Members.Should().ContainSingle();
            table.Members.Single().UserId.Should().Be(1);
        }

        [Fact]
        public void RemoveMember_WithNonMember_ThrowsDomainException()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);

            // Act
            Action act = () => table.RemoveMember(999);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("User is not a member of this expense table");
        }

        [Fact]
        public void RemoveMember_WhenRemovingLastAdmin_ThrowsDomainException()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);

            // Act
            Action act = () => table.RemoveMember(1);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Cannot remove the last remaining admin");
        }

        [Fact]
        public void RemoveMember_WhenAnotherAdminExists_AllowsRemovingOneAdmin()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);
            table.AddMember(2, isAdmin: true);

            // Act
            table.RemoveMember(1);

            // Assert
            table.Members.Should().ContainSingle();
            table.Members.Single().UserId.Should().Be(2);
        }

        [Fact]
        public void Delete_MarksTableAsDeleted()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);

            // Act
            table.Delete();

            // Assert
            table.IsDeleted.Should().BeTrue();
            table.DateDeleted.Should().NotBeNull();
        }

        [Fact]
        public void Delete_WhenAlreadyDeleted_ThrowsDomainException()
        {
            // Arrange
            var table = ExpenseTable.Create("Household", 1);
            table.Delete();

            // Act
            Action act = () => table.Delete();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Expense table is already deleted");
        }
    }
}
