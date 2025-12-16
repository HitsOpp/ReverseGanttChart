# Бэк .net

## Установка и запуск

1. Перейдите в раздел nuget и установите представленные пакетв:

Microsoft.AspNetCore.Authentication.JwtBearer
Microsoft.AspNetCore.Identity
Microsoft.AspNetCore.OpenApi
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Design
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Relational
SwaggerUI.OpenApi
Microsoft.Extensions.Configuration.Json
Microsoft.Extensions.DependencyInjection.Abstractions
Pomelo.EntityFrameworkCore.MySql
Swashbuckle.AspNetCore
System.IdentityModel.Tokens.Jwt
Swashbuckle.AspNetCore
BCrypt
BCrypt.Net-Next
BCrypt.Net-Nex
Microsoft.AspNetCore.Http.Abstractions
Pomelo.EntityFrameworkCore.MySql

2. Или через терминал dotnet add package <название_пакета>:

dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Microsoft.AspNetCore.Http.Abstractions
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Relational
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next  # Самый популярный и актуальный

3. В качетсве бд используется MYSQL

4. После запуска проект будет доступен по адресу написаному в Run, обычно http://localhost:5261/swagger/index.html
