FROM microsoft/dotnet:1.1-runtime-deps

# Install .NET Core
ENV DOTNET_DOWNLOAD_URL https://go.microsoft.com/fwlink/?LinkID=835021

COPY ./Program.cs /usr/src/app/
COPY ./project.json /usr/src/app/
COPY ./project.lock.json /usr/src/app/

WORKDIR /usr/src/app/

RUN apt-get update -qq \
    && apt-get install -y --no-install-recommends -qq \
        curl \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL $DOTNET_DOWNLOAD_URL --output dotnet.tar.gz \
    && mkdir -p /usr/share/dotnet \
    && mkdir -p /usr/src/app \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && /usr/bin/dotnet restore \
    && /usr/bin/dotnet build --configuration Release

CMD /usr/bin/dotnet run --configuration Release