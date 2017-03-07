FROM microsoft/dotnet:1.1-sdk-msbuild

# Create directory for the app source code
WORKDIR /srv

# Copy the source and restore dependencies
COPY bin/Release/netcoreapp1.1/publish/ /srv

ENV ASPNETCORE_ENVIRONMENT Production
ENV HOME /srv

# Expose the port and start the app
EXPOSE 5000
ENTRYPOINT /bin/bash -c "dotnet EShop.dll"
