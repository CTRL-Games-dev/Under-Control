using UnityEngine;

[CreateAssetMenu(fileName = "SO_Itm_Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string DisplayName;
    public string Description;
    public Sprite Icon;
    public Vector2Int Size;
    public int MaxQuantity;
    public GameObject Model;
    public int Value;
}