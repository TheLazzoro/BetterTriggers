mkdir .build\BetterTriggers
dotnet publish GUI\GUI.csproj -c Release --self-contained false --property:PublishDir="..\.build\BetterTriggers" --force -p:GeneratePackageOnBuild=false
dotnet publish Updater\Updater.csproj -c Release --self-contained false --property:PublishDir="..\.build\BetterTriggers" --force -p:GeneratePackageOnBuild=false
