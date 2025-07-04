using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public enum Language {
    English,
    Polish,
    Slaski,
    Ukrainian
}

public static class TextData {
    public static UnityEvent OnLanguageChanged = new UnityEvent();

    public static void ChangeLanguage(Language lang) {
        switch (lang) {
            case Language.English:
                CurrentLanguage = "en";
                break;
            case Language.Polish:
                CurrentLanguage = "pl";
                break;
            case Language.Slaski:
                CurrentLanguage = "sl";
                break;
            case Language.Ukrainian:
                CurrentLanguage = "uk";
                break;
        }
        OnLanguageChanged?.Invoke();
    }

    public static string CurrentLanguage = "en";

    private static string[] _languages;
    public static string[] Languages
    {
        get {
            if (_languages == null) loadLocalizationData();
            return _languages;
        }
    }

    private static Dictionary<string, Dictionary<string, string>> _localizationTable;
    public static Dictionary<string, Dictionary<string, string>> LocalizationTable
    {
        get {
            if (_localizationTable == null) loadLocalizationData();
            return _localizationTable;
        }
    }


    private static void loadLocalizationData() {
        _localizationTable = new Dictionary<string, Dictionary<string, string>>();
        
        TextAsset localizationAsset = Resources.Load<TextAsset>("localization_data");
        if (localizationAsset == null) {
            throw new System.Exception("Localization data not found");
        }

        LocalizationData data = JsonUtility.FromJson<LocalizationData>(localizationAsset.text);
        _languages = data.Languages;

        foreach (LocalizationMapping map in data.Table) {
            _localizationTable[map.Key] = new Dictionary<string, string>();
            foreach (LocalizationValue value in map.Values) {
                _localizationTable[map.Key].Add(value.Lang, value.Value);
            }
        }
    }

    public static Language GetLanguageEnum(string lang) {
        switch (lang) {
            case "en":
                return Language.English;
            case "pl":
                return Language.Polish;
            case "sl":
                return Language.Slaski;
            case "uk":
                return Language.Ukrainian;
            default:
                return Language.English;
        }
    }
}