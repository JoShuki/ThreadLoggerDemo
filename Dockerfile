    #See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreadLoggerDemo.csproj", "."]
RUN dotnet restore "./ThreadLoggerDemo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./ThreadLoggerDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThreadLoggerDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Install tzdata for timezone data
RUN apt-get update && apt-get install -y tzdata

# Set the timezone (e.g., to match the host timezone)
ENV TZ=America/New_York

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreadLoggerDemo.dll"]

# To build the container:
#docker build -t jmaeir/threadloggerdemo .

# To run the container:
#docker run -i -v c:\junk:/log -v /etc/localtime:/etc/localtime:ro jmaeir/threadloggerdemo

# To push the container:
 #docker push jmaeir/threadloggerdemo

 # url to my container
 #https://hub.docker.com/repositories/jmaeir

