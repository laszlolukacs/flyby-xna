# XNA Flight Game #
The XNA Flyby is an XNA powered flight game, originally created in 2012 as a homework for a [course](https://www.aut.bme.hu/Course/VIAUJV01).

## Dependencies ##
* [.NET 4.0 Framework](https://www.microsoft.com/en-us/download/details.aspx?id=17851)
* [Visual Studio 2010](https://msdn.microsoft.com/en-us/library/dd831853(v=vs.100).aspx) with [SP1](https://www.microsoft.com/en-us/download/details.aspx?id=23691) - newer VS versions can be buffed to open XNA projects, [details here](https://mxa.codeplex.com/releases/view/618279)
* [XNA Game Studio 4.0 Refresh](https://www.microsoft.com/en-us/download/details.aspx?id=27598)
* Highly recommended: [NShader](https://nshader.codeplex.com/) extension for VS2010 / [HLSL Tools](https://github.com/tgjones/HlslTools) extension for VS2013/2015

## System Requirements ##
* 1.4 GHz x86 processor
* 512 MB memory
* Shader Model 2.0 (DirectX 9) capable video card with 128 MB memory (practically any video chip should work which is released after 2005)
* Windows XP SP3 or any newer
* [.NET 4.0 Framework](https://www.microsoft.com/en-us/download/details.aspx?id=17851) and [XNA Framework 4.0 Runtime Refresh](https://www.microsoft.com/en-us/download/details.aspx?id=27598)

## Summary of set up
* `git clone git@github.com:laszlolukacs/flyby-xna.git <LOCAL_WORKING_DIR>`
* Install the fonts from the `XnaFlyby.Assets/Fonts` directory
* Open the `XnaFlyby.sln` in Visual Studio 2010

## Basic Usage / How to extend
TBD

## Notes
The game uses the `Reach` *XNA profile* for maximizing compatibility and minimizing system requirements. See the [differences between `Reach` and `Hi-Def` *XNA profiles*](https://blogs.msdn.microsoft.com/shawnhar/2010/03/12/reach-vs-hidef/).

When the game was created I was a happy owner of an IBM ThinkPad T60 machine equipped with the [ATI Mobility Radeon X1400](https://en.wikipedia.org/wiki/List_of_AMD_graphics_processing_units#Mobility_Radeon_X1xxx_Series) GPU. Because of this GPU I was forced to work with the `Reach` *profile* and I quite much stuck with that.

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