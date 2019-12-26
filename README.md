# Flyby #
Flyby is a [MonoGame](http://www.monogame.net/about/) powered flight game, originally started as a homework for a [course](https://www.aut.bme.hu/Course/VIAUJV01) in 2012.

## Dependencies ##
* [.NET Core SDK 3.1.100](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* Windows - Unfortunately [Custom Effects](http://www.monogame.net/documentation/2/?page=Custom_Effects) are not compiling on UNIX systems

## System Requirements ##
* 1.4 GHz x86 processor
* 512 MB memory
* Shader Model 2.0 / OpenGL 2.0 capable video card with 128 MB memory and working OpenGL driver
* [.NET Core Desktop Runtime 3.1.0](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Summary of set up
* `git clone git@github.com:laszlolukacs/flyby-xna.git <LOCAL_WORKING_DIR>`
* Install the fonts from the `./src/Flyby.Application/Contents/Fonts` directory
* Invoke `build.ps1` to compile the application
* Invoke `dotnet ./src/application/Flyby.Application.dll`

## Basic Usage / How to extend
TBD

## Notes
TBD

## Deployment
TBD

## Contribution guidelines
TBD

## Aircraft controls
* **A**/**Z** - Increase/decrease **throttle**, which affects aircraft speed and climbing ability
* **Up**/**Down** arrows - **Pitch** up/down the aircraft
* **Left**/**Right** arrows - **Roll** left/right the aircraft
* Enter - Resets the aircraft to the starting position
* Escape - Quits the game
* (Only during debugging) F2 - Toggles aircraft collision hit boxes

## Quirks, limitations, known issues
* The explorable area is too small, quite easy to fly out of the playable area
* Currently used physics implementation is a way too quick and dirty
* No ability to fire any weapons of the aircraft
* Lack of unit tests