using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string rarity;
    public int power;
    public int dps;
    public int lvl;
    public string desc;
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Show(header, rarity, power, dps, lvl, desc);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}
