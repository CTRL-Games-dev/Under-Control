using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public enum Language
{
    English,
    Polish
}


public static class TextData
{
    public static UnityEvent OnLanguageChanged = new UnityEvent();

    public static void ChangeLanguage(Language lang) {
        switch (lang) {
            case Language.English:
                CurrentLanguage = "en";
                break;
            case Language.Polish:
                CurrentLanguage = "pl";
                break;
        }
        OnLanguageChanged.Invoke();
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
        
        string json = readFromFile(Path.Combine(
            Application.dataPath,
            "!",
            "JSON",
            "localization_data.json")
        );

        LocalizationData data = JsonUtility.FromJson<LocalizationData>(json);
        _languages = data.Languages;

        foreach (LocalizationMapping map in data.Table) {
            _localizationTable[map.Key] = new Dictionary<string, string>();
            foreach (LocalizationValue value in map.Values) {
                _localizationTable[map.Key].Add(value.Lang, value.Value);
            }
        }
    }

    private static string readFromFile(string filePath) {
        if (File.Exists(filePath)) {
            using (StreamReader reader = new StreamReader(filePath)) {
                string json = reader.ReadToEnd();
                reader.Close();
                return json;
            }
        } else {
            Debug.LogWarning("File not found: " + filePath);
            return null;
        }
            
    }

}