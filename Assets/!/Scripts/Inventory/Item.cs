using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string DisplayName;
    public string Description;
    public Sprite Icon;
    public Vector2Int Size;
    public int MaxQuantity;
}