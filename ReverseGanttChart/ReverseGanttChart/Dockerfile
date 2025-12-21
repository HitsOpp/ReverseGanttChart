# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы и публикуем
COPY . ./
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Render предоставляет порт через $PORT
ENV ASPNETCORE_URLS=http://+:$PORT

# Запуск приложения
ENTRYPOINT ["dotnet", "ReverseGanttChart.dll"]
