# 使用 ASP.NET Core 執行環境
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 設定環境變數（確保 HTTPS 能運行）
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/app/LINE_DotNet_API.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="your_password"

# 確保 ASP.NET Core 使用 HTTPS
ENV ASPNETCORE_URLS="https://+:8081;http://+:8080"

# 把憑證 COPY 進 Docker（確保放到 /app/）
COPY LINE_DotNet_API/LINE_DotNet_API.pfx /app/LINE_DotNet_API.pfx

# 使用 .NET SDK 來建置專案
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LINE_DotNet_API/LINE_DotNet_API.csproj", "LINE_DotNet_API/"]
RUN dotnet restore "./LINE_DotNet_API/LINE_DotNet_API.csproj"
COPY . .
WORKDIR "/src/LINE_DotNet_API"
RUN dotnet build "./LINE_DotNet_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 發布應用程式
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LINE_DotNet_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 最終執行階段
FROM base AS final
WORKDIR /app
RUN mkdir -p /app/UploadFiles
COPY --from=publish /app/publish .

# 確保 ASP.NET Core 能夠正確使用 HTTPS 憑證
RUN chmod 644 /app/LINE_DotNet_API.pfx

ENTRYPOINT ["dotnet", "LINE_DotNet_API.dll"]
