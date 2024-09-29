# Flyby
Flyby is a [MonoGame](http://www.monogame.net/about/) powered flight game, originally started as a homework using the [XNA Framework](https://en.wikipedia.org/wiki/Microsoft_XNA) for a university [course](https://www.aut.bme.hu/Course/VIAUJV01) in 2012.

![Screenshot 001](https://github.com/laszlolukacs/flyby-xna/raw/master/docs/screenshots/screenshot001.png)

## Dependencies
* [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or newer
* [MonoGame Content Builder](https://docs.monogame.net/articles/tools/mgcb.html) as [.NET Core global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools), aka `dotnet-mgcb`

## System Requirements
* 1.4 GHz x86 processor
* 512 MB memory
* Shader Model 2.0 / OpenGL 2.0 capable video card with 128 MB memory and working OpenGL driver[1](https://docs.monogame.net/articles/getting_started/platforms.html#desktopgl)
* [.NET Desktop Runtime 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Summary of set up
* `git clone git@github.com:laszlolukacs/flyby-xna.git`
* `dotnet tool install --global dotnet-mgcb --version 3.8.2.1105`
* Invoke `build.ps1` to compile the application
* Invoke `dotnet ./bld/application/Flyby.Application.dll`

The current version utilizing MonoGame and modern .NET is accessible using the `./Flyby.sln` VS solution.

## Aircraft controls
* **A**/**Z** - Increase/decrease **thrust**, which affects aircraft speed and climbing ability
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
* Compiling [Custom Effects](https://docs.monogame.net/articles/getting_started/content_pipeline/custom_effects.html) on Unix systems is still [tricky](https://docs.monogame.net/articles/getting_started/1_setting_up_your_os_for_development_ubuntu.html?tabs=android#setup-wine-for-effect-compilation) as HLSL shader compilation requires a valid Wine installation


## Classic XNA Framework flavour
A version using the classic XNA Framework is still available by opening the `./Flyby.Xna.sln` VS solution. It uses the `MSXNA` conditional build symbol for code branching and the resources exclusive for the classic XNA Content Pipeline have either the `.xnacompat` or the `.xna` suffix in their file names.

###### Dependencies for the classic XNA version
* [Windows 7 SP1](https://prod.support.services.microsoft.com/en-us/windows/create-installation-media-for-windows-99a58364-8c02-206f-aa6f-40c3b507420d)
* [.NET 4.0 Framework](https://www.microsoft.com/en-us/download/details.aspx?id=17851)
* [XNA Game Studio 4.0 Refresh](https://www.microsoft.com/en-us/download/details.aspx?id=27598)
* [Visual Studio 2010](https://msdn.microsoft.com/en-us/library/dd831853(v=vs.100).aspx) with [SP1](https://support.microsoft.com/en-us/topic/description-of-visual-studio-2010-service-pack-1-1f12811e-3826-6728-9f40-b11ee9ae2a0e)
* Install the TrueType fonts from the `./res/Content/Fonts` directory
