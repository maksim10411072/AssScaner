# AssScaner
**Ass**embly **Scan**n**er**, heuristic antivirus for People Playground!
## Installation
1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx)
2. Download dll from [Releases](https://github.com/maksim10411072/AssScaner/releases) 
3. Place it into `BepInEx/plugins/`
## How it works
It uses [Harmony](https://github.com/BepInEx/HarmonyX) to patch [BackgroundItemLoader](https://wiki.studiominus.nl/internalReference/BackgroundItemLoader.html).ProcessModCompilationResult to scan each .dll
in mod directory and prevent mod from loading if there is a dll that [references](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getreferencedassemblies?view=netframework-4.7.2#system-reflection-assembly-getreferencedassemblies)
Facepunch.Steamworks.Win64