FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
LABEL stage=build
WORKDIR /src
COPY ./Source ./Source
COPY ./Website ./Website

RUN dotnet publish ./Website/Website.Server/Website.Server.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ARG aspnetenv=Production

ENV ASPNETCORE_ENVIRONMENT ${aspnetenv}
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Website.Server.dll
