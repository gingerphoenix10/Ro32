<p align="center"><img src="https://github.com/gingerphoenix10/Ro32/raw/main/logo.png" width="75"/></p><h1 align="center">Ro32</h1>
<div align="center">
  
[![Downloads](https://img.shields.io/github/downloads/gingerphoenix10/Ro32/total)](https://github.com/gingerphoenix10/Ro32/releases)
[![Latest Version](https://img.shields.io/github/v/release/gingerphoenix10/Ro32)](https://github.com/gingerphoenix10/Ro32/releases)
[![License](https://img.shields.io/github/license/gingerphoenix10/Ro32)](https://github.com/gingerphoenix10/Ro32/blob/main/LICENSE)
[![Discord](https://img.shields.io/discord/1166129414547980459?logo=discord&logoColor=white&label=discord&color=3b6cff)](https://discord.gg/TZ8qW4HRsG)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/gingerphoenix10/Ro32/dotnet.yml)](https://github.com/gingerphoenix10/Ro32/actions)

</div>
Ro32 is an open-source project built for the Roblox client, Aiming to add more features for devs to interact with the user's device in ways such as opening new windows, moving around Roblox's window itself, and more features available and being worked on.
<br><br>
A lot of the code is used and modified from [Bloxstrap](https://github.com/pizzaboxer/Bloxstrap) by [Pizzaboxer](https://github.com/pizzaboxer), and therefore is also coded in the C# Programming language.
This is the first time i've used C# for anything other than Unity, so the code isn't very good, and it could most likely be improved in hundreds of different ways. If you want to help the development of Ro32 however, You can submit a pull request, and it'll be decided if it will be added (probably)

# Installation & Usage
## Software
To install the Ro32 software, Download the latest version from the [Releases](https://github.com/gingerphoenix10/Ro32/Releases/Latest) tab, and unzip the file. Once unzipped, run Ro32.exe and start Roblox. (Roblox must be opened after running Ro32). The text on the Ro32 application should change to say "Opened", and from there, joining a Ro32 supported game should have their events fired automatically.
## Roblox Studio API
Get the [Ro32 API model](https://create.roblox.com/store/asset/16844513511/Ro32-API) from the Roblox Studio toolbox, and import the ModuleScript into your game. It can be located anywhere, but it's recommended to put the script inside ReplicatedStorage.
Now inside a LocalScript, initialize the module by running `local Ro32 = require(game.ReplicatedStorage.Ro32)`. Change game.ReplicatedStorage to wherever you placed your ModuleScript.
Now from anywhere else in the script, you can run these functions:

### `Ro32.DialogBox(title: string, text: string)`
This will open up a windows dialog box with the inputted title and text.<br><br>
Example:
```Lua
local Ro32 = require(game.ReplicatedStorage.Ro32)
local title = "Window Title"
local text = "Window Text!"
Ro32.DialogBox(title, text)
```

### `Ro32.Minimize()`
Minimizes the Roblox window.<br><br>
Example:
```Lua
local Ro32 = require(game.ReplicatedStorage.Ro32)
Ro32.Minimize()
```
Nothing else to it.
### `Ro32.Maximize()`
Simmilarly to Ro32.Minimize, Ro32.Maximize will maximize the Roblox window if minimized or floating.<br><br>
Example:
```Lua
local Ro32 = require(game.ReplicatedStorage.Ro32)
Ro32.Maximize()
```
### `Ro32.Wallpaper.set(URL: string, FitType: string)`
Will change the user's Background wallpaper to the inputted image from a URL.<br><br> FitType can be set to:<br>`Center` - Centers the image on the user's desktop, and keeps aspect ratio<br>`Fit` - Will rename in an update. Tiles the image for all displays.<br>`Stretch` (or any value other than Center or Fit) - Stretches the image to fit the user's desktop.<br><br>
(Also closes Wallpaper Engine if running.)<br><br>
Example:
```Lua
local Ro32 = require(game.ReplicatedStorage.Ro32)
local ImageURL = "https://github.com/gingerphoenix10/Ro32/raw/main/logo.png"
local FitType = "Stretch"
Ro32.Wallpaper.set(ImageURL, FitType)
```
### `Ro32.Wallpaper.reset()`
Reset's the user's wallpaper to the image it was set to before running Ro32.Wallpaper.set. Reboots Wallpaper Engine if it was running before Ro32.Wallpaper.set.
```Lua
local Ro32 = require(game.ReplicatedStorage.Ro32)
Ro32.Wallpaper.reset()
```
# Building
You will require [Visual Studio](https://visualstudio.microsoft.com/downloads/), and the .NET desktop development workload (download with Visual Studio installer). I don't think there are any other requirements, but don't take my word for it.
Once Visual Studio is installed, Clone the repository, either via Git with "git clone https://github.com/gingerphoenix10/Ro32.git", or by clicking the Download Zip button at the top of the repository, and extracting that.
Now start Visual Studio, and open the "Ro32.sln" file in the git repository you downloaded. Most of the code so far is stored in `Form1.cs`, but will be renamed in a later version.<br>
When you're ready, press the green start button labeled "Ro32" to build and run the application.
Built .exe files will be stored in `(project)\bin\Debug\net8.0-windows\Ro32.exe`
