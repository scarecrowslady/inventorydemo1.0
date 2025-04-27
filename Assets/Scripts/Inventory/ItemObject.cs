using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Default,
    Resource,
    KeyItem,
    Book,
    Food,
    Helmet,
    Weapon,
    Shield,
    Boots,
    Chest,
    Gloves,
    Ring,
    Necklace
}

public enum Attributes
{
    Agility,
    Intellect,
    Stamina,
    Strength,
    Defense,
    Health,
    Attack
}
public abstract class ItemObject : ScriptableObject
{
    public Sprite uiDisplay;

    public bool stackable;

    public ItemType type;
    
    [TextArea(5, 5)]
    public string description;

    public Item data = new Item();

    public int defaultSlotNumber = -1;

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        newItem.itemSlotNumber = defaultSlotNumber; // Assign default slot number
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public string Name;
    
    public ItemType type;
    public int itemSlotNumber;

    public int Id  = -1;
    public ItemBuff[] buffs;

    public Item()
    {
        Name = "";
        Id = -1;

        type = ItemType.Default;
        itemSlotNumber = -1;
    }

    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.data.Id;
        
        type = item.type;
        itemSlotNumber = item.data.itemSlotNumber;

        buffs = new ItemBuff[item.data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max)
            {
                attribute = item.data.buffs[i].attribute
            };
        }
    }
}

[System.Serializable]
public class ItemBuff
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }
    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}