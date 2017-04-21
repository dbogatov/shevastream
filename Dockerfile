FROM microsoft/dotnet:1.1.1-runtime

# Create directory for the app source code
WORKDIR /srv

# Copy the binary
COPY src/bin/release/netcoreapp1.1/publish/ /srv

# Expose the port and start the app
EXPOSE 5555

ENTRYPOINT /bin/bash -c "dotnet shevastream.dll"
