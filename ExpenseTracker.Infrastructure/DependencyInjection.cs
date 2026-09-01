using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.AI;
using ExpenseTracker.Infrastructure.Auth;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using ExpenseTracker.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using System;
using System.Collections.Generic;
using System.Text;

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

            services.AddScoped<IMerchantReader, MerchantRepository>();

            services.AddScoped<IMerchantWriter, MerchantRepository>();

            services.AddScoped<IReceiptWriter, ReceiptRepository>();

            services.AddScoped<IReceiptReader, ReceiptRepository>();  

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IReceiptExtractionService, GeminiReceiptExtractionService>();

            services.AddAuth0Authentication(configuration);

            services.AddReceiptImageStore(configuration);

            services.Configure<GeminiOptions>(configuration.GetSection("Gemini"));

            return services;
        }

        private static IServiceCollection AddReceiptImageStore(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["Storage:Provider"] ?? "AzureBlob";

            if (string.Equals(provider, "Minio", StringComparison.OrdinalIgnoreCase))
            {
                var minioSection = configuration.GetSection("Minio");
                services.Configure<MinioStorageOptions>(minioSection);

                var minioOptions = minioSection.Get<MinioStorageOptions>() ?? new MinioStorageOptions();

                services.AddSingleton<IMinioClient>(_ =>
                {
                    var client = new MinioClient()
                        .WithEndpoint(new Uri(minioOptions.Endpoint))
                        .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
                        .WithSSL(minioOptions.UseSsl);

                    if (!string.IsNullOrWhiteSpace(minioOptions.Region))
                        client = client.WithRegion(minioOptions.Region);

                    return client.Build();
                });

                services.AddScoped<IReceiptImageStore, MinioReceiptImageStore>();
            }
            else
            {
                services.Configure<AzureBlobStorageOptions>(configuration.GetSection("BlobStorage"));
                services.AddScoped<IReceiptImageStore, AzureBlobReceiptImageStore>();
            }

            return services;
        }
    }
}
