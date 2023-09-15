FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app

COPY . .
RUN dotnet publish ./api/Distributed.Workflows.Api -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=base /app/out .
ENTRYPOINT ["dotnet", "Distributed.Workflows.Api.dll"]