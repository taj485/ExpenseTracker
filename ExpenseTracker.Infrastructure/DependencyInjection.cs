using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Auth;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpContextAccessor();

            services.AddScoped<IExpenseWriter, ExpenseRepository>();

            services.AddScoped<IExpenseReader, ExpenseRepository>();

            services.AddScoped<IUserWriter, UserRepository>();

            services.AddScoped<IUserReader, UserRepository>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddAuth0Authentication(configuration);

            return services;
        }
    }
}
