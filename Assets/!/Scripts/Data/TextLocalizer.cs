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
        fixAlignment(_textMeshPro.textInfo);
    }

    public static string GetFormattedString(string input) {
        string formattedString = input.ToString();
        formattedString = formattedString.Replace("%PlayerName%", FormattedStrings.PlayerName);
        return formattedString;
    }


    private void OnEnable() {
        TextData.OnLanguageChanged?.AddListener(UpdateText);
        _textMeshPro.OnPreRenderText += fixAlignment;
        UpdateText();
    }

    private void OnDisable() {
        TextData.OnLanguageChanged?.RemoveListener(UpdateText);
        _textMeshPro.OnPreRenderText -= fixAlignment;
    }


    // ugotowane przez deepseeka, naprawia troche blad
    private void fixAlignment(TMP_TextInfo textInfo)
    {
        if (_textMeshPro.font == null) return;
        
        float referenceBaseline = _textMeshPro.font.faceInfo.baseline;
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Calculate vertical adjustment
            float offset = referenceBaseline - charInfo.baseLine;
            
            // Apply to all four vertices of the character
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j].y += offset;
            }
        }
    }

}
