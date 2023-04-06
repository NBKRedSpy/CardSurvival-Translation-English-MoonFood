using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using 饥荒食物;
using static System.Net.Mime.MediaTypeNames;

namespace 晓月食物
{
    public static class LocalizationStringUtility
    {
        private static SHA1 Sha1 = SHA1.Create();

        private static Dictionary<string, string> Localization;

        /// <summary>
        /// Generates and sets a localization key based on a hash of the DefaultText.
        /// If there is a Localization file loaded, the DefaultText will be set to that key.
        /// </summary>
        /// <param name="localizedString"></param>
        public static void SetLocalizationInfo(this ref LocalizedString localizedString)
        {
            if(Localization == null) Localization = GetLocalizationLookup();
            if (String.IsNullOrEmpty(localizedString.DefaultText)) return;


            //----Set LocalizationKey
            const string prefix = "T-";
            string key = prefix + Convert.ToBase64String(Sha1.ComputeHash(UTF8Encoding.UTF8.GetBytes(localizedString.DefaultText)));

            localizedString.LocalizationKey = key;


            //----Set localized text if available.
            //If there is a Localization lookup loaded, replace the text.
            if(Localization.TryGetValue(key, out string localizedText))
            {
                localizedString.DefaultText = localizedText;
            }

            //Log the keys and current values.  Useful for creating SimpEn.csv entries for the dynamically created cards
            //  in this mod.
            if (MoonFood.LogCardInfo)
            {
                MoonFood.PluginLogger.LogInfo($"{localizedString.LocalizationKey}|{localizedString.DefaultText}");
            }
        }

        /// <summary>
        /// Creates a localization lookup from ./Localization/SimpEn.csv if the game's language is English.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string,string> GetLocalizationLookup()
        {

            try
            {
                if (LocalizationManager.Instance.Languages[LocalizationManager.CurrentLanguage].LanguageName != "English")
                {
                    return new();
                }

                //Check for localization file
                string translationFile = Path.Combine(MoonFood.ModPath, "Localization/SimpEn.csv");

                if (!File.Exists(translationFile))
                {
                    return new();
                }

                Dictionary<string, List<string>> translations = CSVParser.LoadFromPath(translationFile);

                Dictionary<string, string> translationLookup = new();

                foreach (KeyValuePair<string, List<string>> translation in translations)
                {
                    string key = translation.Key;
                    string value = translation.Value[0];

                    //Overwrite if there are multiple entries
                    translationLookup[key] = value;
                }

                return translationLookup;
            }
            catch (Exception ex)
            {
                MoonFood.PluginLogger.LogError($"Error loading SimpEn.csv: {ex}");
                return new();
            }
        }

    }
}
