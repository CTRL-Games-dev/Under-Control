using UnityEngine;

public class ElementalInfo : MonoBehaviour
{
    public Sprite FireBar, IceBar, EarthBar, DefaultBar;
    public Sprite FireIcon, IceIcon, EarthIcon, DefaultIcon;
    public Color FireColor, IceColor, EarthColor, DefaultColor;

    public static ElementalInfo Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public static Sprite GetBarSprite(ElementalType elementalType) {
        switch (elementalType) {
            case ElementalType.Fire:
                return Instance.FireBar;
            case ElementalType.Ice:
                return Instance.IceBar;
            case ElementalType.Earth:
                return Instance.EarthBar;
            default:
                return Instance.DefaultBar;
        }
    }

    public static Sprite GetIconSprite(ElementalType elementalType) {
        switch (elementalType) {
            case ElementalType.Fire:
                return Instance.FireIcon;
            case ElementalType.Ice:
                return Instance.IceIcon;
            case ElementalType.Earth:
                return Instance.EarthIcon;
            default:
                return Instance.DefaultIcon;
        }
    }

    public static Color GetColor(ElementalType elementalType) {
        switch (elementalType) {
            case ElementalType.Fire:
                return Instance.FireColor;
            case ElementalType.Ice:
                return Instance.IceColor;
            case ElementalType.Earth:
                return Instance.EarthColor;
            default:
                return Instance.DefaultColor;
        }
    }
}
