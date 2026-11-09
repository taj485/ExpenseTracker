using ExpenseTracker.Application.Commands.DeleteAccount;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExpenseTracker.Tests.Application.Commands
{
    public class DeleteAccountCommandHandlerTests
    {
        private const string Subject = "auth0|alex";

        private readonly Mock<ICurrentUserService> _currentUserService = new();
        private readonly Mock<IAccountWriter> _accountWriter = new();
        private readonly Mock<IReceiptImageStore> _imageStore = new();
        private readonly Mock<IIdentityProviderAdmin> _identityProviderAdmin = new();
        private readonly Mock<ILogger<DeleteAccountCommandHandler>> _logger = new();
        private readonly DeleteAccountCommandHandler _handler;
        private readonly List<string> _calls = new();

        public DeleteAccountCommandHandlerTests()
        {
            _currentUserService.Setup(x => x.GetCurrentUserSubject()).Returns(Subject);
            _accountWriter.Setup(x => x.DeleteAccountDataAsync(Subject, It.IsAny<CancellationToken>()))
                .Callback(() => _calls.Add("database"))
                .ReturnsAsync(new[] { "tesco.jpg", "lidl.jpg" });
            _imageStore.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((reference, _) => _calls.Add($"photo:{reference}"))
                .Returns(Task.CompletedTask);
            _identityProviderAdmin.Setup(x => x.DeleteUserAsync(Subject, It.IsAny<CancellationToken>()))
                .Callback(() => _calls.Add("auth0"))
                .Returns(Task.CompletedTask);

            _handler = new DeleteAccountCommandHandler(_currentUserService.Object, _accountWriter.Object,
                _imageStore.Object, _identityProviderAdmin.Object, _logger.Object);
        }

        [Fact]
        public async Task Handle_DeletesPhotosAndAuth0User_OnlyAfterTheDatabaseCommit()
        {
            await _handler.Handle(new DeleteAccountCommand(), CancellationToken.None);

            _calls.Should().Equal("database", "photo:tesco.jpg", "photo:lidl.jpg", "auth0");
        }

        [Fact]
        public async Task Handle_WhenAPhotoDeleteFails_LogsItAndStillDeletesTheRest()
        {
            var failure = new InvalidOperationException("blob store down");
            _imageStore.Setup(x => x.DeleteAsync("tesco.jpg", It.IsAny<CancellationToken>())).ThrowsAsync(failure);

            await _handler.Handle(new DeleteAccountCommand(), CancellationToken.None);

            _imageStore.Verify(x => x.DeleteAsync("lidl.jpg", It.IsAny<CancellationToken>()), Times.Once);
            _identityProviderAdmin.Verify(x => x.DeleteUserAsync(Subject, It.IsAny<CancellationToken>()), Times.Once);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true),
                failure, It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAuth0DeleteFails_ThrowsExternalServiceExceptionWithTheUserMessage()
        {
            _identityProviderAdmin.Setup(x => x.DeleteUserAsync(Subject, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Auth0 is down"));

            var act = () => _handler.Handle(new DeleteAccountCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ExternalServiceException>()
                .WithMessage("Couldn't delete your account. Please try again.");
        }

        [Fact]
        public async Task Handle_SecondCallWithNothingLeftInTheDatabase_StillFinishesTheAuth0Step()
        {
            _accountWriter.Setup(x => x.DeleteAccountDataAsync(Subject, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<string>());

            await _handler.Handle(new DeleteAccountCommand(), CancellationToken.None);

            _imageStore.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _identityProviderAdmin.Verify(x => x.DeleteUserAsync(Subject, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
