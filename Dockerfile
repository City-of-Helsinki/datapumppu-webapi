#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./WebAPI/WebAPI.csproj", "./"]
RUN dotnet restore "WebAPI.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "./WebAPI/WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./WebAPI/WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM node:16 AS node-builder

WORKDIR /components/
COPY ./WebAPI/WebComponents/Decision /components/
RUN npm install
RUN npm run build

WORKDIR /components/decision
COPY ./WebAPI/WebComponents/Decision /components/decision
RUN npm install
RUN npm run build

WORKDIR /components/meeting
COPY ./WebAPI/WebComponents/Meeting /components/meeting
RUN npm install
RUN npm run build

FROM base AS final
RUN mkdir -p /app/ScriptFiles/components
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node-builder /components/decision/dist/bundle.js ./ScriptFiles/components/decision.js
COPY --from=node-builder /components/meeting/dist/bundle.js ./ScriptFiles/components/meeting.js
ENTRYPOINT ["dotnet", "WebAPI.dll"]