ARG REPO=mcr.microsoft.com/dotnet/runtime

# Installer image
FROM arm64v8/buildpack-deps:bookworm-curl AS installer

# Retrieve ASP.NET Core
# https://github.com/dotnet/dotnet-docker/tree/main
RUN aspnetcore_version=8.0.6 \
    && curl -fSL --output aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$aspnetcore_version/aspnetcore-runtime-$aspnetcore_version-linux-arm64.tar.gz \
    && aspnetcore_sha512='9ed12847e404a0a4fdd8fca33a9a787c5ac2e6745d23821c7890f731f2f8f5682e7f9166b2764b13b612b08e091c71e13359b68acc11bcf990afdef1d42f6473' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -oxzf aspnetcore.tar.gz ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

# ASP.NET Core image
# https://hub.docker.com/_/microsoft-dotnet-runtime/
FROM $REPO:8.0.6-bookworm-slim-arm64v8

# ASP.NET Core version
ENV ASPNET_VERSION=8.0.6
ENV ASPNETCORE_ENVIRONMENT=StagingDocker
ENV ASPNETCORE_URLS=http://+:8000
ENV GUNICORN_CMD_ARGS="--timeout 60 -k gevent -b 0.0.0.0:5003"
EXPOSE 5003
ENV SERVER_PORT 8000
ENV SERVER_HOST 0.0.0.0

COPY --from=installer ["/shared/Microsoft.AspNetCore.App", "/usr/share/dotnet/shared/Microsoft.AspNetCore.App"]

VOLUME [ "/data" ]

COPY API-Files/Staging/ ./App
WORKDIR /App
# ENTRYPOINT ["dotnet", "exec", "/App/IOBootstrap-NET.Application.dll"]
ENTRYPOINT ["/bin/bash"]
