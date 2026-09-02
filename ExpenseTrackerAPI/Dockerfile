# Build from the repo root: docker build -f ExpenseTrackerAPI/Dockerfile .
# The context has to be the root so the referenced projects are reachable.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore against the project files alone, so editing code does not
# invalidate the restore layer.
COPY ExpenseTracker.Domain/ExpenseTracker.Domain.csproj ExpenseTracker.Domain/
COPY ExpenseTracker.Application/ExpenseTracker.Application.csproj ExpenseTracker.Application/
COPY ExpenseTracker.Infrastructure/ExpenseTracker.Infrastructure.csproj ExpenseTracker.Infrastructure/
COPY ExpenseTrackerAPI/ExpenseTrackerAPI.csproj ExpenseTrackerAPI/
RUN dotnet restore ExpenseTrackerAPI/ExpenseTrackerAPI.csproj

COPY ExpenseTracker.Domain/ ExpenseTracker.Domain/
COPY ExpenseTracker.Application/ ExpenseTracker.Application/
COPY ExpenseTracker.Infrastructure/ ExpenseTracker.Infrastructure/
COPY ExpenseTrackerAPI/ ExpenseTrackerAPI/
RUN dotnet publish ExpenseTrackerAPI/ExpenseTrackerAPI.csproj \
    -c Release --no-restore -o /app/publish

# The aspnet image already runs as the non-root "app" user and listens on 8080.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ExpenseTrackerAPI.dll"]
