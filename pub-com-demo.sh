#!/bin/sh

CSPROJ_PATH="./HiveAssistant/ComDemo/ComDemo.csproj"

# Publish as self-contained for Linux-x64 (change RID as needed)
dotnet publish "$CSPROJ_PATH" -c Release -r linux-musl-x64 --self-contained true -o /etc/hive/com-demo

# Or as framework-dependent (needs dotnet runtime installed)
# dotnet publish -c Release -o ./publish

echo "Publish complete. Output is in ./publish"