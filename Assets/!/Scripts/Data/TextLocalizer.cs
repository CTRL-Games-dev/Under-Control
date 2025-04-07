using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalizer : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    private string _key = string.Empty;
    public string Key {
        get => _key;
        set {
            _key = value;
            if (gameObject.activeSelf) {
                UpdateText();
            }
        }
    }

    private void Awake() {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _key = _textMeshPro.text;

        if (gameObject.activeSelf) {
            UpdateText();
            TextData.OnLanguageChanged?.RemoveListener(UpdateText);
            TextData.OnLanguageChanged?.AddListener(UpdateText);
        }
    }


    public void UpdateText() {        
        try {
            _ = TextData.LocalizationTable[Key][TextData.CurrentLanguage];
        } catch (KeyNotFoundException) {
            _textMeshPro.text = GetFormattedString(Key);
            return;
        } 

        _textMeshPro.text = GetFormattedString(TextData.LocalizationTable[Key][TextData.CurrentLanguage].ToString());
    }

    public static string GetFormattedString(string input) {
        string formattedString = input.ToString();
        formattedString = formattedString.Replace("%PlayerName%", FormattedStrings.PlayerName);
        return formattedString;
        
    }


    private void OnEnable() {
        TextData.OnLanguageChanged?.AddListener(UpdateText);
        UpdateText();
    }

    private void OnDisable() {
        TextData.OnLanguageChanged?.RemoveListener(UpdateText);
    }
}
