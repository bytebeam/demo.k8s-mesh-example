FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /app

COPY . ./
RUN dotnet publish -r linux-musl-x64 -c Release -o out src/CustomersApi

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "CustomersApi.dll"]