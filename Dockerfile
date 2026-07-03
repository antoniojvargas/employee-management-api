FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY EmployeeManagement.slnx ./
COPY src/EmployeeManagement.Domain/*.csproj src/EmployeeManagement.Domain/
COPY src/EmployeeManagement.Application/*.csproj src/EmployeeManagement.Application/
COPY src/EmployeeManagement.Infrastructure/*.csproj src/EmployeeManagement.Infrastructure/
COPY src/EmployeeManagement.Api/*.csproj src/EmployeeManagement.Api/
COPY tests/EmployeeManagement.Tests/*.csproj tests/EmployeeManagement.Tests/
RUN dotnet restore src/EmployeeManagement.Api/EmployeeManagement.Api.csproj

COPY src/ src/
RUN dotnet publish src/EmployeeManagement.Api/EmployeeManagement.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
