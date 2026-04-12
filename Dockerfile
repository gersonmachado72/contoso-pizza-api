# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos do projeto
COPY ContosoPizza.csproj .
RUN dotnet restore

# Copiar todo o código e compilar
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Estágio 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar os arquivos publicados do estágio de build
COPY --from=build /app/publish .

# Expor a porta que o Render vai usar
EXPOSE 8080
ENV PORT=8080

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "ContosoPizza.dll"]
