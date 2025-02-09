FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install dependencies for Python
RUN apt-get update && apt-get install -y \
    python3.12 \
    python3.12-venv \
    python3.12-dev \
    && rm -rf /var/lib/apt/lists/*

# Set Python 3.12 as default
RUN ln -sf /usr/bin/python3.12 /usr/bin/python3
RUN ln -sf /usr/bin/python3.12 /usr/bin/python

# Create virtual environment
RUN python -m venv /app/venv
ENV PATH="/app/venv/bin:$PATH"

# Copy requirements file and install dependencies
COPY requirements.txt /app/requirements.txt
RUN pip install --no-cache-dir -r /app/requirements.txt

# Install PyTorch
RUN pip install torch torchvision torchaudio

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Glosy.csproj", "./"]
RUN dotnet restore "Glosy.csproj"
COPY . .
WORKDIR "/src/Glosy"
RUN dotnet build "Glosy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Glosy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Glosy.dll"]