using Azure.Storage.Blobs;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Analyser;
using ExpenseTracker.Infrastructure.Auth;
using ExpenseTracker.Infrastructure.Messaging;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using ExpenseTracker.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace ExpenseTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ExpenseTrackerDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpContextAccessor();

            services.AddScoped<IExpenseWriter, ExpenseRepository>();

            services.AddScoped<IExpenseReader, ExpenseRepository>();

            services.AddScoped<IUserWriter, UserRepository>();

            services.AddScoped<IUserReader, UserRepository>();

            services.AddScoped<IExpenseTableWriter, ExpenseTableRepository>();

            services.AddScoped<IExpenseTableReader, ExpenseTableRepository>();

            services.AddScoped<IReceiptWriter, ReceiptRepository>();

            services.AddScoped<IReceiptReader, ReceiptRepository>();

            services.AddScoped<IReceiptExtractionJobWriter, ReceiptExtractionJobRepository>();

            services.AddScoped<IReceiptExtractionJobReader, ReceiptExtractionJobRepository>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddAuth0Authentication(configuration);

            services.Configure<AzureBlobStorageOptions>(configuration.GetSection("BlobStorage"));

            services.Configure<KafkaOptions>(configuration.GetSection("Kafka"));

            services.Configure<ReceiptAnalyserOptions>(configuration.GetSection("ReceiptAnalyser"));

            // One client per process: it pools connections internally and is expensive to rebuild.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AzureBlobStorageOptions>>().Value;

                if (string.IsNullOrWhiteSpace(options.ConnectionString))
                    throw new InvalidOperationException("Azure Blob Storage is not configured.");

                return new BlobServiceClient(options.ConnectionString);
            });

            services.AddScoped<IReceiptImageStore, AzureBlobReceiptImageStore>();

            // Confluent's producer is thread-safe and batches in the background — share one.
            services.AddSingleton<IReceiptExtractionRequestPublisher, KafkaReceiptExtractionPublisher>();

            services.AddSingleton<Auth0TokenProvider>();

            services.AddHttpClient<IReceiptExtractionResultReader, ReceiptAnalyserClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ReceiptAnalyserOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}
