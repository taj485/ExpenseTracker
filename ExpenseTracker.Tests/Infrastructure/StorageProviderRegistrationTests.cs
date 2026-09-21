using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Storage;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Tests.Infrastructure
{
    /// <summary>
    /// Receipt storage is chosen by configuration, so the binding itself is the thing worth
    /// guarding — nothing else in the codebase names a concrete image store.
    /// </summary>
    public class StorageProviderRegistrationTests
    {
        private static IReceiptImageStore ResolveStoreFor(Dictionary<string, string?> settings)
        {
            settings["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var provider = new ServiceCollection()
                .AddInfrastructure(configuration)
                .BuildServiceProvider();

            using var scope = provider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IReceiptImageStore>();
        }

        [Fact]
        public void MinioProviderResolvesTheMinioStore()
        {
            var store = ResolveStoreFor(new Dictionary<string, string?>
            {
                ["Storage:Provider"] = "Minio",
                ["Minio:Endpoint"] = "http://localhost:9000",
                ["Minio:AccessKey"] = "minioadmin",
                ["Minio:SecretKey"] = "minioadmin",
                ["Minio:BucketName"] = "receipt-images",
            });

            store.Should().BeOfType<MinioReceiptImageStore>();
        }

        [Fact]
        public void AzureBlobProviderResolvesTheAzureStore()
        {
            var store = ResolveStoreFor(new Dictionary<string, string?>
            {
                ["Storage:Provider"] = "AzureBlob",
            });

            store.Should().BeOfType<AzureBlobReceiptImageStore>();
        }

        [Fact]
        public void TheProviderNameIsCaseInsensitive()
        {
            var store = ResolveStoreFor(new Dictionary<string, string?>
            {
                ["Storage:Provider"] = "minio",
                ["Minio:Endpoint"] = "http://localhost:9000",
                ["Minio:AccessKey"] = "minioadmin",
                ["Minio:SecretKey"] = "minioadmin",
            });

            store.Should().BeOfType<MinioReceiptImageStore>();
        }

        [Fact]
        public void AzureBlobRemainsTheDefaultWhenNoProviderIsConfigured()
        {
            var store = ResolveStoreFor(new Dictionary<string, string?>());

            store.Should().BeOfType<AzureBlobReceiptImageStore>();
        }
    }
}
