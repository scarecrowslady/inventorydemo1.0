using UnityEngine;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

//create an inventory (backend)
//create multiple for various use cases
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public ItemDatabaseObject database;
    public Inventory Container;

    public InventoryObject inventoryToolbar;
    public InventoryObject inventoryMain;
    public InventoryObject inventoryEquipment;

    #region On Enable
    private void OnEnable() // Ensure that the inventory container is always initialized
    {
        // Initialize the container if it hasn't been initialized
        if (Container == null)
        {
            Container = new Inventory();
        }

        // Initialize slot numbers if they haven't been assigned yet
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i] == null) // Ensure the slot is not null
            {
                Container.Items[i] = new InventorySlot(); // Create a new InventorySlot if necessary
            }

            if (Container.Items[i] != null && Container.Items[i].slotNumber == -1)
            {
                Container.Items[i].slotNumber = i; // Assign the slot number
                Debug.Log($"Slot {i} initialized.");
            }
        }
    }
    #endregion

    //add items to the inventory based on database ids
    public bool AddItem(Item _item, int _amount)
    {
        // First try to add the item to the toolbar inventory.
        InventorySlot slot = inventoryToolbar.FindItemOnInventory(_item);

        // If an existing stack is found and the item is stackable, add to that stack.
        if (slot != null && database.GetItem[_item.Id].stackable)
        {
            slot.AddAmount(_amount);
            return true;
        }

        // If there's space in the toolbar, add the item there.
        if (inventoryToolbar.EmptySlotCount > 0)
        {
            inventoryToolbar.SetEmptySlot(_item, _amount);
            return true;
        }

        // If the toolbar is full, try to add to the main inventory.
        slot = inventoryMain.FindItemOnInventory(_item);
        if (slot != null && database.GetItem[_item.Id].stackable)
        {
            slot.AddAmount(_amount);
            return true;
        }

        // If the main inventory has space, add the item there.
        if (inventoryMain.EmptySlotCount > 0)
        {
            inventoryMain.SetEmptySlot(_item, _amount);
            return true;
        }

        // Both inventories are full, cannot add item.
        // ------------------------> add a method here that pops up inventory full message
        return false;
    }

    public bool LoadInAddedItems(Item _item, int _amount)
    {
        // Get the corresponding ItemObject from the database to access the type
        ItemObject itemObject = database.GetItem[_item.Id];

        // Set the ItemType based on the ItemObject's type
        _item.type = itemObject.type;

        // First try to add the item to the toolbar inventory.
        InventorySlot slot = inventoryToolbar.FindItemOnInventory(_item);

        // If an existing stack is found and the item is stackable, add to that stack.
        if (slot != null && database.GetItem[_item.Id].stackable)
        {
            slot.AddAmount(_amount);
            return true;
        }

        // If there's space in the toolbar, add the item there.
        if (inventoryToolbar.EmptySlotCount > 0)
        {
            inventoryToolbar.SetEmptySlot(_item, _amount);
            return true;
        }

        // Now check if the item can go into the equipment inventory
        // This assumes certain items are allowed only in equipment slots
        // Implement this function to check if the item is equipment and check the slots as well
        if (IsEquipmentItem(_item))
        {
            // Check if the equipment slot is specified (via slotNumber) and valid
            if (_item.itemSlotNumber >= 0 && _item.itemSlotNumber < inventoryEquipment.Container.Items.Length)
            {
                InventorySlot slotSlot = inventoryEquipment.Container.Items[_item.itemSlotNumber];
                if (slotSlot.item.Id == -1) // Check if the slot is empty
                {
                    slotSlot.UpdateSlot(_item, _amount, _item.itemSlotNumber);
                    return true;
                }
            }
            // If slot number is invalid or slot is occupied, fall back to regular inventory logic
        }

        // If the toolbar is full, try to add to the main inventory.
        slot = inventoryMain.FindItemOnInventory(_item);
        if (slot != null && database.GetItem[_item.Id].stackable)
        {
            slot.AddAmount(_amount);
            return true;
        }

        // If the main inventory has space, add the item there.
        if (inventoryMain.EmptySlotCount > 0)
        {
            inventoryMain.SetEmptySlot(_item, _amount);
            return true;
        }

        // Both inventories are full, cannot add item.
        // add a method here that pops up inventory full message
        return false;
    }

    // Helper function to determine if an item should be placed in equipment slots
    private bool IsEquipmentItem(Item _item)
    {
        // Check if the item type is one of the equipment-related types
        ItemType itemType = _item.type;

        // List all equipment-related item types
        return itemType == ItemType.Helmet ||
               itemType == ItemType.Weapon ||
               itemType == ItemType.Shield ||
               itemType == ItemType.Boots ||
               itemType == ItemType.Chest ||
               itemType == ItemType.Gloves ||
               itemType == ItemType.Ring ||
               itemType == ItemType.Necklace;
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if (Container.Items[i].item.Id <= -1)
                    counter++;
            }
            return counter;
        }
    }
    public InventorySlot FindItemOnInventory(Item _item)
    {
        for(int i = 0;i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.Id == _item.Id)
            {
                return Container.Items[i];
            }
        }
        return null;
    }

    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.Id <= -1)
            {
                Container.Items[i].UpdateSlot(_item, _amount, _item.itemSlotNumber);
                Debug.Log("On clicking items, slots that are updated include: " + _item.itemSlotNumber);
                return Container.Items[i];
            }
        }

        //set up functionality for full inventory

        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if(item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject)) 
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount, item2.slotNumber);
            item2.UpdateSlot(item1.item, item1.amount, item1.slotNumber);
            item1.UpdateSlot(temp.item, temp.amount, temp.slotNumber);
        }
    }

    //public void RemoveItem(Item _item)
    //{
    //    for (int i = 0; i < Container.Items.Length; i++)
    //    {
    //        if (Container.Items[i].item == _item)
    //        {
    //            Container.Items[i].UpdateSlot(null, 0, -1);
    //        }
    //    }
    //}

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
}

[System.Serializable]
public class Inventory : IEnumerable<InventorySlot>
{
    public InventorySlot[] Items = new InventorySlot[10];

    public IEnumerator<InventorySlot> GetEnumerator()
    {
        // Use yield return to iterate over the Items array
        foreach (var slot in Items)
        {
            yield return slot;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].RemoveItem();
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];

    //[System.NonSerialized]
    public UserInterface parent;
    [System.NonSerialized]
    public GameObject slot;

    public Item item;
    public int amount;
    public int slotNumber;

    public ItemObject ItemObject
    {
        get
        {
            if(item.Id >= 0)
            {
                return parent.inventory.database.GetItem[item.Id];
            }
            return null;
        }
    }

    public InventorySlot()
    {
        item = new Item();
        amount = 0;
        slotNumber = -1;
    }
    public InventorySlot(Item _item, int _amount, int _slotNumber)
    {
        item = _item;
        amount = _amount;
        slotNumber = _slotNumber;
    }
    //updating slots
    public void UpdateSlot(Item _item, int _amount, int _slotNumber)
    {
        item = _item;
        amount = _amount;
        slotNumber = _slotNumber;
    }
    //remove an item entirely
    public void RemoveItem()
    {
        item = new Item();
        amount = 0;
        //slotNumber = -1;
    }
    //add an item by value
    public void AddAmount (int value)
    {
        amount += value;
    }
    //check whether you can place the item in the slot
    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if(AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
        {
            return true;
        }

        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i])
            {
                return true;
            }
        }
        return false;
    }
}