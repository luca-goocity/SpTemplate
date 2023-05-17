FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/Service/SpTemplate.Service.First/SpTemplate.Service.First.csproj", "Src/Service/SpTemplate.Service.First/"]
RUN dotnet restore "Src/Service/SpTemplate.Service.First/SpTemplate.Service.First.csproj"
COPY . .
WORKDIR "/src/Src/Service/SpTemplate.Service.First"
RUN dotnet build "SpTemplate.Service.First.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpTemplate.Service.First.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpTemplate.Service.First.dll"]
