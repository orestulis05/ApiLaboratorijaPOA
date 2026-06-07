FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ApiLaboratorija/ApiLaboratorija.csproj ApiLaboratorija/
RUN dotnet restore ApiLaboratorija/ApiLaboratorija.csproj

COPY . .
RUN dotnet publish ApiLaboratorija/ApiLaboratorija.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ApiLaboratorija.dll"]