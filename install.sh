#!/bin/bash

# Install Brew, ASSUMING CURL IS INSTALLED
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install Build Dependencides
brew install nuget dotnet mono unzip git
# ^ SEE IF MONO CAN REPLACE `msbuild
# ^ SEE IF UNZUP CAN REPLACE `7z`

# COMMON VARIABLES

# START

# Checkout commit

# Fetch history
git fetch --prune --unshallow --recurse-submodules=no

# Checkout Head Ref
if [[ github.head_ref -ne '' ]]; then
  git checkout -b ${{ github.head_ref }} || git checkout ${{ github.head_ref }}
fi

# Set up .NET environment

# Install NuGet packages
nuget restore LiveSplit/LiveSplit.sln

# Build LiveSplit
mono LiveSplit/LiveSplit.sln # Minimal Verbosity, NoWarm, & Release Build

# Run tests
# Currently requires vstest.console.exe

# Create a folder for the Server component artifacts
mkdir LiveSplit.Server
mv LiveSplit/bin/Release/Components/LiveSplit.Server.dll LiveSplit.Server
mv LiveSplit/bin/Release/Components/Noesis.Javascript.dll LiveSplit.Server

# Upload the Server component as an artifact

# Upload the Counter component as an artifact

# Clean up build folder
mkdir LiveSplit/bin/Release/Resources
cp /LiveSplit/Resources/SplitsFile.ico LiveSplit/bin/Release/Resources
cp /LiveSplit/Resources/LayoutFile.ico LiveSplit/bin/Release/Resources
cd LiveSplit/bin/Release
# Remove all files from `66`-`72`

# Upload build as an artifact

# Upload build to LiveSplit.github.io
if [[ github.repository -e 'LiveSplit/LiveSplit' && github.ref -e 'refs/heads/master' ]]; then
  7z LiveSplitDevBuild.zip LiveSplit/bin/Release
  git config --global user.email "action@github.com"
  git config --global user.name "GitHub Action"
  git clone -q --branch master --single-branch "https://github.com/LiveSplit/LiveSplit.github.io.git"
  cd LiveSplit.github.io
  git checkout -q --orphan artifacts
  git reset
  mv ../LiveSplitDevBuild.zip . -force
  git add LiveSplitDevBuild.zip
  git commit -m "Add Development Build"
  git push -q --force https://action:${{ secrets.DEV_BUILD_UPLOAD }}@github.com/LiveSplit/LiveSplit.github.io.git
fi
