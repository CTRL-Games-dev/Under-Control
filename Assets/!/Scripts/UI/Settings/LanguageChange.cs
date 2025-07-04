using TMPro;
using UnityEngine;

public class LanguageChange : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _languageDropdown;

    private void Start() {
        _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        TextData.OnLanguageChanged.AddListener(OnLanguageChangeEvent);
    }

    public void ChangeLanguage(string languageCode) {
        switch (languageCode) {
            case "English":
                TextData.ChangeLanguage(Language.English);
                break;
            case "Polski":
                TextData.ChangeLanguage(Language.Polish);
                break;
        }
    }

    private void OnLanguageChanged(int index) {
        string selectedLanguage = _languageDropdown.options[index].text;
        ChangeLanguage(selectedLanguage);
    }

    private void OnLanguageChangeEvent() {
        _languageDropdown.value = (int)TextData.GetLanguageEnum(TextData.CurrentLanguage);
    }
}
