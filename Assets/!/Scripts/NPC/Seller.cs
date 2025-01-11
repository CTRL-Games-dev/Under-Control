using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopItem
{
    public int Quantity = 1;
    public Item Item;
    public int Price = 1;
}

[CreateAssetMenu(fileName = "SellerItems", menuName = "ScriptableObject/SellerItems", order = 1)]
public class SellerItems : ScriptableObject
{
    public List<ShopItem> ShopItems;
}

public class Seller : NPC
{
    [SerializeField] private readonly String _interactString;
    [SerializeField] public SellerItems ItemsToSell;
    private PlayerController _player;

    public UnityEvent SelectedItem;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public override string GetInteractString()
    {
        return _interactString;
    }

    public override void StartInteraction()
    {
        StartedInteraction.Invoke();
    }

    public override void StopInteraction()
    {
        StoppedInteraction.Invoke();
    }

    private void ReduceItemQuantity(ShopItem shopItem, int quantity)
    {
        shopItem.Quantity -= quantity;
        if(shopItem.Quantity <= 0) {
            ItemsToSell.ShopItems.Remove(shopItem);
        }
    }

    public Item SellItem(int index, out bool succeded)
    {
        if(ItemsToSell.ShopItems.Count < index - 1)
        {
            succeded = false;
            return null;
        }
        ShopItem shopItem = ItemsToSell.ShopItems[index];
        if(shopItem.Price > _player.gold)
        {
            succeded = false;
            return null;
        }

        _player.gold -= shopItem.Price;
        ReduceItemQuantity(shopItem, 1);
        succeded = true;
        return shopItem.Item;
    }

    public Item SellItem(int index, out bool succeded, int quantity)
    {
        if(ItemsToSell.ShopItems.Count < index - 1)
        {
            succeded = false;
            return null;
        }

        ShopItem shopItem = ItemsToSell.ShopItems[index];
        if(shopItem.Quantity < quantity)
        {
            succeded = false;
            return null;
        }

        if(quantity >= 0){
            Debug.LogWarning("Requested quantity of \"Shop item\" was >= 0");
            succeded = false;
            return null;
        }

        int goldNeeded = shopItem.Price*quantity;
        if(goldNeeded > _player.gold)
        {
            succeded = false;
            return null;
        }

        _player.gold -= goldNeeded;
        ReduceItemQuantity(shopItem, quantity);
        succeded = true;
        return shopItem.Item;
    }

    public Item SellItem(ShopItem shopItem, out bool succeded, int quantity)
    {
        if(!ItemsToSell.ShopItems.Contains(shopItem))
        {
            Debug.LogWarning("Item tried to be sold, yet seller does not have it");
            succeded = false;
            return null;
        }

        if(shopItem.Price > _player.gold)
        {
            succeded = false;
            return null;
        }

        if(quantity >= 0){
            Debug.LogWarning("Requested quantity of \"Shop item\" was >= 0");
            succeded = false;
            return null;
        }

        int goldNeeded = shopItem.Price*quantity;
        if(goldNeeded > _player.gold)
        {
            succeded = false;
            return null;
        }

        _player.gold -= goldNeeded;
        ReduceItemQuantity(shopItem, 1);
        succeded = true;
        return shopItem.Item;
    }
}