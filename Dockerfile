FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
ARG BUILDPLATFORM

# Install NativeAOT build prerequisites
#RUN apt-get update \
#    && apt-get install -y --no-install-recommends \
#       clang zlib1g-dev

WORKDIR /src
COPY ["src/Elastic.Markdown/Elastic.Markdown.csproj", "src/Elastic.Markdown/Elastic.Markdown.csproj"]
COPY ["src/docs-builder/docs-builder.csproj", "src/docs-builder/docs-builder.csproj"]
COPY ["docs/docs.csproj", "docs/docs.csproj"]
COPY ["docs-builder.sln", "docs-builder.sln"]
RUN dotnet restore "docs-builder.sln"
COPY . .
WORKDIR "/src"
RUN dotnet build "src/docs-builder/docs-builder.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "src/docs-builder/docs-builder.csproj" -c Release -o /app/publish \
    #--runtime alpine-x64 \
    --self-contained true \
    /p:PublishTrimmed=true \
    /p:PublishSingleFile=true \
    /p:PublishAot=false \
    -a $TARGETARCH

FROM --platform=$BUILDPLATFORM base AS final
ARG TARGETARCH
ARG BUILDPLATFORM

# create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

# impersonate into the new user
USER dotnetuser
WORKDIR /app

COPY --from=publish /app/publish .
CMD chmod +x docs-builder

ENTRYPOINT ["./docs-builder"]
