
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


COPY *.sln .
COPY ReverseGanttChart/*.csproj ./ReverseGanttChart/
RUN dotnet restore


COPY ReverseGanttChart/. ./ReverseGanttChart/
WORKDIR /src/ReverseGanttChart
RUN dotnet publish -c Release -o /app


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./


ENV ASPNETCORE_URLS=http://+:$PORT


ENTRYPOINT ["dotnet", "ReverseGanttChart.dll"]
