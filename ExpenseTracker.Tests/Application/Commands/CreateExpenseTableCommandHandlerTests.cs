using ExpenseTracker.Application.Commands.CreateExpenseTable;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class CreateExpenseTableCommandHandlerTests
    {
        private readonly Mock<IExpenseTableWriter> _mockExpenseTableWriter;
        private readonly Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private readonly User _currentUser;
        private readonly CreateExpenseTableCommandHandler _handler;

        public CreateExpenseTableCommandHandlerTests()
        {
            _mockExpenseTableWriter = new Mock<IExpenseTableWriter>();
            _mockCurrentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUser = User.Create("auth0|test-user");
            _currentUser.Id = 1;
            _mockCurrentUserProvider.Setup(x => x.GetOrProvisionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_currentUser);
            var validator = new CreateExpenseTableValidator();

            _handler = new CreateExpenseTableCommandHandler(_mockExpenseTableWriter.Object, validator, _mockCurrentUserProvider.Object);
        }

        [Fact]
        public async Task Handle_WithValidName_CreatesTableWithCurrentUserAsAdmin()
        {
            // Arrange
            var command = new CreateExpenseTableCommand("Household");

            _mockExpenseTableWriter.Setup(x => x.AddAsync(It.IsAny<ExpenseTable>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _mockExpenseTableWriter.Verify(x => x.AddAsync(It.Is<ExpenseTable>(t =>
                t.Name == "Household" &&
                t.CreatedByUserId == _currentUser.Id &&
                t.Members.Single().IsAdmin), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        public async Task Handle_WithEmptyName_ThrowsValidationException(string name)
        {
            // Arrange
            var command = new CreateExpenseTableCommand(name);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Name*");

            _mockExpenseTableWriter.Verify(
                x => x.AddAsync(It.IsAny<ExpenseTable>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
