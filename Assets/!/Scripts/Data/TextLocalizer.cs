using TMPro;
using Unity.AppUI.UI;
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
        
        string playerName = "jogn";
        string formatted = TextData.LocalizationTable[Key][TextData.CurrentLanguage].ToString();
        formatted = string.Format(formatted, playerName);
        _textMeshPro.text = formatted;
    }


    private void OnEnable() {
        TextData.OnLanguageChanged?.AddListener(UpdateText);
        UpdateText();
    }

    private void OnDisable() {
        TextData.OnLanguageChanged?.RemoveListener(UpdateText);
    }
}
