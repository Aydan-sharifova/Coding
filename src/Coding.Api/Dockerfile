FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source
COPY ["Coding.sln", "."]
COPY ["src/Coding.Domain/Coding.Domain.csproj", "src/Coding.Domain/"]
COPY ["src/Coding.Application/Coding.Application.csproj", "src/Coding.Application/"]
COPY ["src/Coding.Infrastructure/Coding.Infrastructure.csproj", "src/Coding.Infrastructure/"]
COPY ["src/Coding.Api/Coding.Api.csproj", "src/Coding.Api/"]
RUN dotnet restore "src/Coding.Api/Coding.Api.csproj"
COPY . .
RUN dotnet publish "src/Coding.Api/Coding.Api.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app
COPY --from=build --chown=appuser:appgroup /app/publish .
USER appuser
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Coding.Api.dll"]
