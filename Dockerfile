FROM microsoft/dotnet:3.0-sdk AS build-env
WORKDIR /app
RUN pwd
# Copy everything and build
COPY . ./
RUN dotnet restore

RUN dotnet publish -c Release -o out
# RUN pwd
# RUN ls -la
# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.0 AS runtime
WORKDIR /app

COPY --from=build-env /app/out/ .

RUN pwd
RUN ls -la
RUN find "$PWD" -type f

ENTRYPOINT ["dotnet", "KubernetesJobBroker.dll"]
