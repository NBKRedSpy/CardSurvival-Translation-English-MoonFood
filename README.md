# MoonFood Translation
This is an unofficial English translation of guilyou492's MoonFood mod.

The initial version is machine translated, but is open for community help.

https://www.nexusmods.com/cardsurvivaltropicalisland/mods/54

This repository will be removed when or if the author integrates an English translation.

To use, download the .zip from the releases page and install as normal.

Description is in the NexusModDescription-English.txt file.


# English Translations
To change any translations, find the text in the ./Localization/SimpEn.csv and change the text as desired.

# Creating New Translations
To completely create a ./Localization/SimpEn.csv file from scratch, do the following:

## JSON files
Use the tool at [CardSurvival-Localization](https://github.com/NBKRedSpy/CardSurvival-Localization).  Read the instructions on how to use.
The tool will create the ./Localization/SimpEn.csv file from the .json files. 

## DLL Dynamic Card Keys
The mod has a built in ability to create a translation key for every created object inside the .DLL.  In order to handle dynamically created cards, the key is a SHA1 hash based on the item's DefaultText.

The config has a LogCardInfo option to output the keys and current text to the BepInEx output.

### Process
* Start the game with this mod installed.
* Exit the game
* In the `Plugin.MoonFood.cfg` change LogCardInfo to true
* Run the game again.
* Exit the game again.

The `\BepInEx\LogOutput.log` will include translation keys in pipe delimited format.

```
[Info   :  MoonFood] T-uTkzEGfQjO47T1xpaDx41uUoTEo=|美味！
[Info   :  MoonFood] T-XFdZqo+bxrChd/dXIvpMOJoUMLU=|什锦肉串
```

Translate those entries, and add them to the ./Localization/SimpEn.csv in CSV format:

```
T-uTkzEGfQjO47T1xpaDx41uUoTEo=,Some Translation, 美味！
T-XFdZqo+bxrChd/dXIvpMOJoUMLU=,Some Other Translation,什锦肉串
```

 

