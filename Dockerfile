# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["PruebaDeDesempeño.Web/PruebaDeDesempeño.Web.csproj", "PruebaDeDesempeño.Web/"]
RUN dotnet restore "PruebaDeDesempeño.Web/PruebaDeDesempeño.Web.csproj"

# Copiar el resto del código y compilar
COPY . .
WORKDIR "/src/PruebaDeDesempeño.Web"
RUN dotnet build "PruebaDeDesempeño.Web.csproj" -c Release -o /app/build

# Etapa 2: Publish
FROM build AS publish
RUN dotnet publish "PruebaDeDesempeño.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "PruebaDeDesempeño.Web.dll"]
