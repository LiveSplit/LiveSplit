#!/bin/bash

# Install Brew
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install Build Dependencides
brew install nuget dotnet mono unzip
# ^ SEE IF MONO CAN REPLACE `msbuild
# ^ SEE IF UNZUP CAN REPLACE `7z`

# Nuget packages

# Build LiveSplit

# Run tests

# Clean up build folder
