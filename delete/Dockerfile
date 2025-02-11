# # 基於 ASP.NET Core Runtime 映像
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# WORKDIR /app
# EXPOSE 8080

# # 使用 .NET SDK 構建映像
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY . .
# RUN dotnet restore "LINE_DotNet_API/LINE_DotNet_API.csproj"
# RUN dotnet publish "LINE_DotNet_API/LINE_DotNet_API.csproj" -c Release -o /app/build

# # 最終映像
# FROM base AS final
# WORKDIR /app
# RUN mkdir -p /app/UploadFiles
# COPY --from=build /app/build .
# ENTRYPOINT ["dotnet", "LINE_DotNet_API.dll"]
