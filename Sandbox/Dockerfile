FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Project.Sandbox/Project.Sandbox.csproj", "Project.Sandbox/"]
RUN dotnet restore "Project.Sandbox/Project.Sandbox.csproj"
COPY . .
WORKDIR "/src/Project.Sandbox"
RUN dotnet build "Project.Sandbox.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Project.Sandbox.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.Sandbox.dll"]
