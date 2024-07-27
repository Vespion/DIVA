FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-azurelinux3.0-distroless-aot AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0-aot AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGET_RUNTIME=linux-x64
WORKDIR /src
COPY ["src/Gateway/Gateway.csproj", "src/Gateway/"]
RUN dotnet restore "src/Gateway/Gateway.csproj" -r $TARGET_RUNTIME
COPY . .
WORKDIR "/src/src/Gateway"
RUN dotnet build "Gateway.csproj" -r $TARGET_RUNTIME -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.csproj" -r $TARGET_RUNTIME -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["VespionSoftworks.DIVA.Gateway"]
