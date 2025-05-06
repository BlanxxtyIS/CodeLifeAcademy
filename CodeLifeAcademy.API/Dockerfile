# 1. Используем SDK-образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2. Копируем и восстанавливаем зависимости
COPY . .
RUN dotnet restore

# 3. Публикуем приложение
RUN dotnet publish CodeLifeAcademy.API/CodeLifeAcademy.API.csproj -c Release -o /app/publish

# 4. Используем рантайм-образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 5. Открываем порт (на всякий)
EXPOSE 80

# 6. Точка входа
ENTRYPOINT ["dotnet", "CodeLifeAcademy.API.dll"]