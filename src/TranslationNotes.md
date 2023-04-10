# Code Change Notes
There are places where .SetLocalizationInfo() could possibly just be called once instead of in every if block and sets a LocalizedString.DefaultText property.  But mostly doing it out of caution and to simplify the translation.

联动精耕细作() is a good example.


# Concerns
* There is code that is setting LocalizationKey to blank explicitly.

Example:  
```		cardData.CardName.DefaultText = string.Concat(植株.CardName, "植株");
		cardData.CardName.LocalizationKey = "";
```

* There is a "Dummy" LocalizationKey that is reused.  I've seen this in other mods, but not sure why it is set.  

```localizedString.LocalizationKey = "Guil-更多水果_Dummy";```

* Compound ActionName && ActionName.LocalizationKey.  Not sure why a distinct LocalizationKey is not used.

```if (_Action.ActionName == "高级炼金" && _Action.ActionName.LocalizationKey == "Guil-炼金")```



# General Notes

There is a reoccurring pattern  in these mods where they search for a duplicate localization key and an Action name.  
I'm not sure why they do both.  I would think that a unique key would be correct.  Either way, it is subverting the purpose
of a localization key.

Code	File	Line	Column
		if (_Action.ActionName == "高级炼金" && _Action.ActionName.LocalizationKey == "Guil-炼金")	C:\src\CardSurvival\English Card Translations\CardSurvival-Translation-English-MoonFood\src\饥荒食物\MoonFood.cs	1105	29
		if (_Action.ActionName == "容器合并" && _Action.ActionName.LocalizationKey == "Guil-炼金")	C:\src\CardSurvival\English Card Translations\CardSurvival-Translation-English-MoonFood\src\饥荒食物\MoonFood.cs	1109	29


However, not sure why this is done, unless there was a misunderstanding what the Localization Key is used for?  There are other parts of code that also assign the same "Dummy" localization key.

Perhaps that since the base came won't overwrite DefaultText from the language database, they felt free to use it as a discriminator.

Either way, I hope I'm not missing something...


