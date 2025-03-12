using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public Tooltip tooltip;

    public void Awake()
    {
        current = this;
    }

    public static void Show(string header, string rarity, int power, int dps, int lvl, string desc)
    {
        current.tooltip.SetText(header, rarity, power, dps, lvl, desc);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
