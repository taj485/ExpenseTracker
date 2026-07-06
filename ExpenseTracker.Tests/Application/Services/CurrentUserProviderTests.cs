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
        private readonly CurrentUserProvider _provider;

        public CurrentUserProviderTests()
        {
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockUserReader = new Mock<IUserReader>();
            _mockUserWriter = new Mock<IUserWriter>();
            _provider = new CurrentUserProvider(_mockCurrentUserService.Object, _mockUserReader.Object, _mockUserWriter.Object);
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