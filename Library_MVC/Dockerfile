FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Library_MVC/Library_MVC.csproj", "Library_MVC/"]
RUN dotnet restore "Library_MVC/Library_MVC.csproj"
COPY . .
WORKDIR "/src/Library_MVC"
RUN dotnet build "Library_MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Library_MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library_MVC.dll"]
