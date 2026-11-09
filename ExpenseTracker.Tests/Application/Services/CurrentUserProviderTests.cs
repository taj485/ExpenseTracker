using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Tests.Application.Services
{
    public class CurrentUserProviderTests
    {
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IUserReader> _mockUserReader;
        private readonly Mock<IUserWriter> _mockUserWriter;
        private readonly Mock<IDeletedAccountReader> _mockDeletedAccountReader;
        private readonly CurrentUserProvider _provider;

        public CurrentUserProviderTests()
        {
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockUserReader = new Mock<IUserReader>();
            _mockUserWriter = new Mock<IUserWriter>();
            _mockDeletedAccountReader = new Mock<IDeletedAccountReader>();
            _provider = new CurrentUserProvider(_mockCurrentUserService.Object, _mockUserReader.Object, _mockUserWriter.Object,
                _mockDeletedAccountReader.Object);
        }

        [Fact]
        public async Task GetOrProvisionAsync_WhenUserExistsWithSameEmail_ReturnsUserWithoutWriting()
        {
            // Arrange
            var existingUser = User.Create("auth0|existing", "person@example.com");
            _mockCurrentUserService.Setup(x => x.GetCurrentUserSubject()).Returns("auth0|existing");
            _mockCurrentUserService.Setup(x => x.GetCurrentUserEmail()).Returns("person@example.com");
            _mockUserReader.Setup(x => x.GetByUserIdpAsync("auth0|existing", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _provider.GetOrProvisionAsync(CancellationToken.None);

            // Assert
            result.Should().Be(existingUser);
            _mockUserWriter.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUserWriter.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetOrProvisionAsync_WhenUserDoesNotExist_CreatesAndSavesNewUser()
        {
            // Arrange
            _mockCurrentUserService.Setup(x => x.GetCurrentUserSubject()).Returns("auth0|new-user");
            _mockCurrentUserService.Setup(x => x.GetCurrentUserEmail()).Returns("new@example.com");
            _mockUserReader.Setup(x => x.GetByUserIdpAsync("auth0|new-user", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _provider.GetOrProvisionAsync(CancellationToken.None);

            // Assert
            result.Subject.Should().Be("auth0|new-user");
            result.Email.Should().Be("new@example.com");
            _mockUserWriter.Verify(x => x.AddAsync(It.Is<User>(u => u.Subject == "auth0|new-user"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task S7_GetOrProvisionAsync_WhenSubjectWasDeleted_ThrowsUnauthorizedAndCreatesNoUser()
        {
            // Arrange
            _mockCurrentUserService.Setup(x => x.GetCurrentUserSubject()).Returns("auth0|deleted");
            _mockUserReader.Setup(x => x.GetByUserIdpAsync("auth0|deleted", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _mockDeletedAccountReader.Setup(x => x.IsDeletedAsync("auth0|deleted", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = () => _provider.GetOrProvisionAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _mockUserWriter.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task S5_GetOrProvisionAsync_ForANewSubjectWithADeletedAccountsEmail_CreatesAFreshUser()
        {
            // Arrange
            _mockCurrentUserService.Setup(x => x.GetCurrentUserSubject()).Returns("auth0|alex-again");
            _mockCurrentUserService.Setup(x => x.GetCurrentUserEmail()).Returns("alex@example.com");
            _mockUserReader.Setup(x => x.GetByUserIdpAsync("auth0|alex-again", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _mockDeletedAccountReader.Setup(x => x.IsDeletedAsync("auth0|alex", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _provider.GetOrProvisionAsync(CancellationToken.None);

            // Assert
            result.Subject.Should().Be("auth0|alex-again");
            _mockUserWriter.Verify(x => x.AddAsync(It.Is<User>(u => u.Subject == "auth0|alex-again"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrProvisionAsync_WhenUserDoesNotExistAndNoEmailClaim_CreatesUserWithNullEmail()
        {
            // Arrange
            _mockCurrentUserService.Setup(x => x.GetCurrentUserSubject()).Returns("auth0|new-user");
            _mockCurrentUserService.Setup(x => x.GetCurrentUserEmail()).Returns((string?)null);
            _mockUserReader.Setup(x => x.GetByUserIdpAsync("auth0|new-user", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _provider.GetOrProvisionAsync(CancellationToken.None);

            // Assert
            result.Email.Should().BeNull();
        }

        [Fact]
        public async Task GetOrProvisionAsync_WhenStoredEmailDiffersFromCurrentEmail_UpdatesEmail()
        {
            // Arrange
            var existingUser = User.Create("auth0|existing", "old@example.com");
            _mockCurrentUserService.Setup(x => x.GetCurrentUserSubject()).Returns("auth0|existing");
            _mockCurrentUserService.Setup(x => x.GetCurrentUserEmail()).Returns("new@example.com");
            _mockUserReader.Setup(x => x.GetByUserIdpAsync("auth0|existing", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _provider.GetOrProvisionAsync(CancellationToken.None);

            // Assert
            result.Email.Should().Be("new@example.com");
            _mockUserWriter.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Email == "new@example.com")), Times.Once);
        }
    }
}