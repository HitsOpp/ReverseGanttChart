# Backend (.NET)

##  Описание

Данный проект представляет собой backend-приложение на **ASP.NET Core** с использованием **JWT-аутентификации**, **Entity Framework Core** и базы данных **MySQL**. В проекте настроена документация API через **Swagger**.

---

##  Установка и запуск

### 1️ Установка необходимых NuGet-пакетов

Перейдите в **NuGet Package Manager** и установите следующие пакеты:

* `Microsoft.AspNetCore.Authentication.JwtBearer`
* `Microsoft.AspNetCore.Identity`
* `Microsoft.AspNetCore.OpenApi`
* `Microsoft.AspNetCore.Http.Abstractions`
* `Microsoft.EntityFrameworkCore`
* `Microsoft.EntityFrameworkCore.Design`
* `Microsoft.EntityFrameworkCore.Tools`
* `Microsoft.EntityFrameworkCore.Relational`
* `Microsoft.Extensions.Configuration.Json`
* `Microsoft.Extensions.DependencyInjection.Abstractions`
* `Pomelo.EntityFrameworkCore.MySql`
* `System.IdentityModel.Tokens.Jwt`
* `Swashbuckle.AspNetCore`
* `BCrypt.Net-Next` 

---

### 2️ Установка пакетов через терминал

Вы также можете установить все зависимости с помощью CLI:

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Microsoft.AspNetCore.Http.Abstractions
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Relational
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Swashbuckle.AspNetCore
dotnet add package BCrypt.Net-Next
```

---

### 3️ База данных

В качестве базы данных используется **MySQL**.

Провайдер для работы с EF Core:

* `Pomelo.EntityFrameworkCore.MySql`

Убедитесь, что строка подключения корректно указана в `appsettings.json`.

---

### 4️ Запуск проекта

После запуска приложение будет доступно по адресу (по умолчанию):

```
http://localhost:5261/swagger/index.html
```

Swagger предоставляет удобный интерфейс для тестирования и просмотра API.

---

##  Готово!

Проект готов к использованию и дальнейшей разработке.

Если порт отличается — ориентируйтесь на значение, указанное в консоли при запуске (`Run`).

