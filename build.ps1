Write-Output "Performing a clean build of the Flyby Game"

# Cleans the results of any previous builds
Remove-Item -Recurse -Force ".\bld" -ErrorAction Ignore

dotnet tool restore
dotnet restore .\Flyby.sln
dotnet publish .\src\Flyby.Application\Flyby.Application.csproj --configuration Release --output .\bld\application
Compress-Archive -Path .\bld\application\* -DestinationPath .\bld\Flyby.zip
Write-Output "Work complete"
