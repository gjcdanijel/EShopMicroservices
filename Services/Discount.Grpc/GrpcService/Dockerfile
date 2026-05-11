FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../Services/Discount.Grpc/GrpcService1/GrpcService1.csproj", "../Services/Discount.Grpc/GrpcService1/"]
RUN dotnet restore "../Services/Discount.Grpc/GrpcService1/GrpcService1.csproj"
COPY . .
WORKDIR "/src/../Services/Discount.Grpc/GrpcService1"
RUN dotnet build "./GrpcService1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GrpcService1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrpcService1.dll"]
