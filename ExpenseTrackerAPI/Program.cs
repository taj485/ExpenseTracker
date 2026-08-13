using ExpenseTracker.Application;
using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Infrastructure;
using ExpenseTrackerAPI.Hubs;
using ExpenseTrackerAPI.Messaging;
using ExpenseTrackerAPI.Middleware;
using FluentValidation;
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
              // SignalR needs credentials on the handshake; safe here because the origin is explicit.
              .AllowCredentials()
              .WithExposedHeaders("Content-Disposition"));
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSignalR()
    .AddJsonProtocol(opts =>
        opts.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddHostedService<ReceiptExtractionCompletedConsumer>();

var app = builder.Build();

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
app.MapHub<ReceiptExtractionHub>("/hubs/receipt-extraction");

app.Run();

public partial class Program { }
