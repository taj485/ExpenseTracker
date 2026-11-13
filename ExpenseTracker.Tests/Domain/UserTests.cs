using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;

namespace ExpenseTracker.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Create_WithSubjectAndEmail_ReturnsUser()
        {
            // Act
            var user = User.Create("auth0|abc123", "person@example.com");

            // Assert
            user.Subject.Should().Be("auth0|abc123");
            user.Email.Should().Be("person@example.com");
        }

        [Fact]
        public void Create_WithoutEmail_ReturnsUserWithNullEmail()
        {
            // Act
            var user = User.Create("auth0|abc123");

            // Assert
            user.Email.Should().BeNull();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        public void Create_WithEmptySubject_ThrowsDomainException(string subject)
        {
            // Act
            Action act = () => User.Create(subject);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Subject is required");
        }

        [Fact]
        public void UpdateEmail_ChangesStoredEmail()
        {
            // Arrange
            var user = User.Create("auth0|abc123", "old@example.com");

            // Act
            user.UpdateEmail("new@example.com");

            // Assert
            user.Email.Should().Be("new@example.com");
        }

        [Fact]
        public void Create_WithMixedCaseEmail_StoresLowercase()
        {
            // Act
            var user = User.Create("auth0|abc123", "Person@Example.COM");

            // Assert
            user.Email.Should().Be("person@example.com");
        }

        [Fact]
        public void UpdateEmail_WithMixedCaseEmail_StoresLowercase()
        {
            // Arrange
            var user = User.Create("auth0|abc123", "old@example.com");

            // Act
            user.UpdateEmail("New@Example.COM");

            // Assert
            user.Email.Should().Be("new@example.com");
        }

        [Fact]
        public void Create_StartsWithNoAiReceiptConsent()
        {
            var user = User.Create("auth0|abc123");

            user.AiReceiptConsentVersion.Should().BeNull();
            user.AiReceiptConsentGivenAt.Should().BeNull();
        }

        [Fact]
        public void GiveAiReceiptConsent_ForANewVersion_ReplacesTheOlderOne()
        {
            var user = User.Create("auth0|abc123");
            var first = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var second = first.AddMonths(6);
            user.GiveAiReceiptConsent(1, first);

            user.GiveAiReceiptConsent(2, second);

            user.AiReceiptConsentVersion.Should().Be(2);
            user.AiReceiptConsentGivenAt.Should().Be(second);
        }
    }
}