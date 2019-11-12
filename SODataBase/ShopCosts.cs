using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopCosts", menuName = "Data/ShopCosts")]
public class ShopCosts : Data
{
    [System.Serializable]
    public class ItemCost
    {
        public string itemName;
        public int cost;
    }

    [System.Serializable]
    public class ItemType
    {
        public GameEnums.ShopItemType shopItemType;
        public List<ItemCost> items;
    }

    public List<ItemType> itemsInShop;

    public int ItemsCost(string itemName)
    {
        for (int i = 0; i < itemsInShop.Count; i++)
        {
            for (int j = 0; j < itemsInShop[i].items.Count; j++)
            {
                if(itemsInShop[i].items[j].itemName == itemName)
                {
                    return itemsInShop[i].items[j].cost;
                }
            }
        }

        Debug.Log($"<color=red> Shop does not contain {itemName} </color>");
        return 0;
    }
}
