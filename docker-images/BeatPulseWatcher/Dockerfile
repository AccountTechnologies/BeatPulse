FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS sdk-with-node
ENV NODE_VERSION 8.12.0
ENV NODE_DOWNLOAD_SHA 3df19b748ee2b6dfe3a03448ebc6186a3a86aeab557018d77a0f7f3314594ef6
RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

FROM sdk-with-node AS updated-npm
RUN npm i -g npm

FROM updated-npm AS build
WORKDIR /src
COPY ["BeatPulseWatcher/BeatPulseWatcher.csproj", "BeatPulseWatcher/"]
RUN dotnet restore "BeatPulseWatcher/BeatPulseWatcher.csproj"
COPY . .
WORKDIR "/src/BeatPulseWatcher"
RUN dotnet build "BeatPulseWatcher.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BeatPulseWatcher.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BeatPulseWatcher.dll"]