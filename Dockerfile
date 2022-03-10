FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 53324

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Todos.API/Todos.API.csproj", "Todos.API/"]
COPY ["Todos.Models/Todos.Models.csproj", "Todos.Models/"]
COPY ["Todos.Utils/Todos.Utils.csproj", "Todos.Utils/"]
COPY ["Todos.Repositories/Todos.Repositories.csproj", "Todos.Repositories/"]
RUN dotnet restore "Todos.API/Todos.API.csproj"
COPY . .
WORKDIR "/src/Todos.API"
RUN dotnet build "Todos.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Todos.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Todos.API.dll"]
