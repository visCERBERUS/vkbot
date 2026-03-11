FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем всё остальное и публикуем
COPY . ./
RUN dotnet publish -c Release -o out

# Сборка финального образа
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80

ENTRYPOINT ["dotnet", "VkLeakBot.dll"]
