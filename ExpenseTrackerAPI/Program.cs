using ExpenseTracker.Application;
using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTrackerAPI.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

const string SpaCorsPolicy = "SpaCorsPolicy";

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddCors(options =>
{
    options.AddPolicy(SpaCorsPolicy, policy =>
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigin"]!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Content-Disposition"));
});
// Behind nginx and the Cloudflare tunnel, which terminate TLS upstream: without
// this the app sees every request as plain http and UseHttpsRedirection below
// would try to redirect traffic that is already secure.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // The proxy is another container on a dynamic Docker network address, so the
    // default known-proxy allowlist would silently discard the headers. Safe here
    // because the API port is bound to loopback and is not exposed through the tunnel.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Off by default so tests and tooling never reach for a database; the
// container turns it on so a fresh volume comes up with schema and seed data.
if (app.Configuration.GetValue<bool>("Database:AutoMigrate"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<ExpenseTrackerDbContext>().Database.Migrate();
}

// Must run before anything that inspects the scheme or the client IP.
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(SpaCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
