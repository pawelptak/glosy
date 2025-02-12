FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install Python 3.11 and required dependencies
RUN apt-get update && apt-get install -y \
    ffmpeg \
    python3-pip \
    python3.11-venv \
    && rm -rf /var/lib/apt/lists/*
    #python3.11 \
    #python3.11-venv \
    #python3.11-dev \

# Set Python 3.11 as default
#RUN ln -sf /usr/bin/python3.11 /usr/bin/python3
#RUN ln -sf /usr/bin/python3.11 /usr/bin/python

# Create virtual environment
RUN python3 -m venv /app/venv
ENV PATH="/app/venv/bin:$PATH"

# Copy requirements file and install dependencies
COPY requirements.txt /app/requirements.txt
RUN pip install --no-cache-dir -r /app/requirements.txt

# Install PyTorch
RUN pip install torch torchvision torchaudio

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ENV TTS_HOME=/app/PythonScripts/TTS/models
COPY ["Glosy.csproj", "."]
RUN dotnet restore "./Glosy.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Glosy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Glosy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# This ensures that the environment variable is set correctly at runtime
ARG VERSION
ENV APP_VERSION=${VERSION} \
    TTS_HOME=/app/PythonScripts/TTS/models


ENTRYPOINT ["dotnet", "Glosy.dll"]
