using UnityEngine;

public class ElementalInfo : MonoBehaviour
{
    public Sprite FireBar, IceBar, EarthBar, DefaultBar;
    public Sprite FireIcon, IceIcon, EarthIcon, DefaultIcon;
    public Color FireColor, IceColor, EarthColor, CharmColor, LifeStealColor, ManaStealColor, StunColor, HealthColor, ManaColor, ArmorColor, LightAttackColor, HeavyAttackColor, CardsColor, EvoHealthColor, EvoManaColor, DefaultColor;

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
            case ElementalType.Charming:
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
            case ElementalType.Charming:
                return Instance.CharmColor;
            case ElementalType.LifeSteal:
                return Instance.LifeStealColor;
            case ElementalType.ManaSteal:
                return Instance.ManaStealColor;
            case ElementalType.Stun:
                return Instance.StunColor;
            case ElementalType.Health:
                return Instance.HealthColor;
            case ElementalType.Mana:
                return Instance.ManaColor;
            case ElementalType.Armor:
                return Instance.ArmorColor;
            case ElementalType.LightAttack:
                return Instance.LightAttackColor;
            case ElementalType.HeavyAttack:
                return Instance.HeavyAttackColor;
            case ElementalType.EvoHealth:
                return Instance.EvoHealthColor;
            case ElementalType.EvoMana:
                return Instance.EvoManaColor;
            case ElementalType.EvoCards:
                return Instance.CardsColor;

            case ElementalType.None:
            default:
                return Instance.DefaultColor;
        }
    }
}
