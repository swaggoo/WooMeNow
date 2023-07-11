FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY WooMeNow.API.csproj .
RUN dotnet restore "WooMeNow.API.csproj"
COPY . .
RUN dotnet publish "WooMeNow.API.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT [ "dotnet", "WooMeNow.API.dll" ]