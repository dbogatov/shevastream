FROM microsoft/dotnet:1.1-sdk-msbuild

# Create directory for the app source code
RUN mkdir -p /usr/src/
WORKDIR /usr/src/

# Copy the source and restore dependencies
COPY . /usr/src/

RUN dotnet restore

# Expose the port and start the app
EXPOSE 5000
CMD [ "dotnet", "run" ]
