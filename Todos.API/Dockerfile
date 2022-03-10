FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["todos-example/todos-example.csproj", "todos-example/"]
RUN dotnet restore "todos-example/todos-example.csproj"
COPY . .
WORKDIR "/src/todos-example"
RUN dotnet build "todos-example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "todos-example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "todos-example.dll"]
