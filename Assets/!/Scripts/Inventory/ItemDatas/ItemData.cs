using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string DisplayName;
    [TextArea]
    public string Description;
    public Sprite Icon;
    public Vector2Int Size;
    public int MaxQuantity;
    public GameObject Model;
}