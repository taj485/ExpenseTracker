using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;

namespace ExpenseTracker.Tests.Domain
{
    public class DeletedAccountTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithoutASubject_ThrowsDomainException(string? subject)
        {
            var act = () => DeletedAccount.Create(subject!, DateTime.UtcNow);

            act.Should().Throw<DomainException>().WithMessage("Subject is required");
        }

        [Fact]
        public void Create_StoresTheSubjectAndAUtcTime()
        {
            var at = new DateTime(2026, 11, 9, 12, 0, 0, DateTimeKind.Utc);

            var account = DeletedAccount.Create("auth0|alex", at);

            account.Subject.Should().Be("auth0|alex");
            account.DeletedAt.Should().Be(at);
            account.DeletedAt.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void Create_WithAnUnspecifiedTime_MarksItUtc()
        {
            var at = new DateTime(2026, 11, 9, 12, 0, 0, DateTimeKind.Unspecified);

            var account = DeletedAccount.Create("auth0|alex", at);

            account.DeletedAt.Kind.Should().Be(DateTimeKind.Utc);
            account.DeletedAt.Ticks.Should().Be(at.Ticks);
        }
    }
}
