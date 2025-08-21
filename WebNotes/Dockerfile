# Stage 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Копируем .csproj и зависимости
COPY WebNotes/*.csproj ./WebNotes/
COPY *.sln ./

# Восстанавливаем зависимости
RUN dotnet restore WebNotes/WebNotes.csproj

# Копируем весь код
COPY . .

# Собираем и публикуем
WORKDIR /app/WebNotes
RUN dotnet publish -c Release -o out

# Stage 2: Запуск
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=build /app/WebNotes/out .

# Render требует: слушать на порту из переменной PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-5000}

# Запускаем основной проект
ENTRYPOINT ["dotnet", "WebNotes.dll"]