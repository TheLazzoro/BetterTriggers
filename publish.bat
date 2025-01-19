mkdir .build\BetterTriggers
dotnet publish -r win-x64 -c Release --framework net6.0-windows --self-contained false --property:PublishDir="..\.build\BetterTriggers" --force
