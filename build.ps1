dotnet restore
dotnet publish .\src\Flyby.Application\Flyby.Application.csproj --configuration Release --output .\bld\application
Compress-Archive -Path .\bld\application\* -DestinationPath .\bld\Flyby.zip
Write-Output "Work complete"
