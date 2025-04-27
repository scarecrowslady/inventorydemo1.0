using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour,IDataPersistence
{
    public InventoryObject inventoryToolbar;
    public InventoryObject inventoryEquipment;
    public InventoryObject inventoryMain;

    public int deathCount = 0;
    public GameObject deathCountObj;
    public TextMeshProUGUI deathCountText;

    public List<GameObject> pickupableObjects = new List<GameObject>();

    private void Awake()
    {
        deathCountText = deathCountObj.GetComponent<TextMeshProUGUI>();
    }

    public void Start()
    {
        LoadGame();
    }

    private void Update()
    {
        deathCountText.text = "" + deathCount;
    }

    #region Tracking Variables -- Death, Adding Information
    //adding items on click
    public void AddItem(GameObject other)
    {
        var item = other.GetComponent<GroundItem>();
        if(item)
        {
            Item _item = new Item(item.item);
            if(inventoryToolbar.AddItem(_item, 1))
            {
                //Destroy(other.gameObject);
                other.gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayerDeath()
    {
        deathCount++;
    }
    #endregion

    #region Load and Save Data
    public void LoadData(GameData data)
    {
        this.deathCount = data.deathCount;

        //clear current inventory
        inventoryToolbar.Clear();
        inventoryEquipment.Clear();
        inventoryMain.Clear();

        //load new inventory
        foreach (var slot in data.toolbarData.Container)
        {            
            if (slot.item != null && inventoryToolbar.database.GetItem.ContainsKey(slot.item.Id))
            {
                var item = inventoryToolbar.database.GetItem[slot.item.Id];
                Item newItem = new Item(item);
                newItem.type = item.type;
                inventoryToolbar.LoadInAddedItems(newItem, slot.amount);
            }
            else
            {
                Debug.LogWarning($"Invalid item ID {slot.item?.Id} in toolbarData");
            }
        }
        foreach (var slot in data.equipmentData.Container)
        {
            if (slot.item != null && inventoryEquipment.database.GetItem.ContainsKey(slot.item.Id))
            {
                var item = inventoryEquipment.database.GetItem[slot.item.Id];
                Item newItem = new Item(item);
                newItem.type = item.type;
                inventoryEquipment.LoadInAddedItems(newItem, slot.amount);
            }
            else
            {
                Debug.LogWarning($"Invalid item ID {slot.item?.Id} in equipmentData");
            }
        }
        foreach (var slot in data.playerinventoryData.Container)
        {
            if (slot.item != null && inventoryMain.database.GetItem.ContainsKey(slot.item.Id))
            {
                var item = inventoryMain.database.GetItem[slot.item.Id];
                Item newItem = new Item(item);
                newItem.type = item.type;
                inventoryMain.LoadInAddedItems(newItem, slot.amount);
            }
            else
            {
                Debug.LogWarning($"Invalid item ID {slot.item?.Id} in playerinventoryData");
            }
        }
    }
    public void SaveData(GameData data)
    {
        data.deathCount = this.deathCount;

        //clear saved inventory data
        data.toolbarData.Container.Clear();
        data.equipmentData.Container.Clear();
        data.playerinventoryData.Container.Clear();

        // Save new inventory for the toolbar
        foreach (var slot in inventoryToolbar.Container)
        {
            if (slot.item != null && slot.item.Id != -1)  // Only save valid items
            {
                data.toolbarData.Container.Add(new InventorySlot(slot.item, slot.amount, slot.slotNumber));
            }
        }

        // Save new inventory for equipment
        foreach (var slot in inventoryEquipment.Container)
        {
            if (slot.item != null && slot.item.Id != -1)  // Only save valid items
            {
                data.equipmentData.Container.Add(new InventorySlot(slot.item, slot.amount, slot.slotNumber));
            }
        }

        // Save new inventory for main player inventory
        foreach (var slot in inventoryMain.Container)
        {
            if (slot.item != null && slot.item.Id != -1)  // Only save valid items
            {
                data.playerinventoryData.Container.Add(new InventorySlot(slot.item, slot.amount, slot.slotNumber));
            }
        }
    }
    #endregion

    #region Loading, Saving, New Game States
    public void LoadGame()
    {
        DataPersistanceManager.instance.LoadGame();
    }
    public void SaveGame()
    {
        DataPersistanceManager.instance.SaveGame();
    }
    public void NewGame()
    {
        deathCount = 0;

        inventoryToolbar.Clear();
        inventoryEquipment.Clear();
        inventoryMain.Clear();

        foreach (var pickupableObject in pickupableObjects)
        {
            gameObject.SetActive(true);
        }

        DataPersistanceManager.instance.NewGame();
    }
    #endregion

    #region exiting
    private void OnApplicationQuit()
    {
        DataPersistanceManager.instance.SaveGame();

        //inventoryToolbar.Container.Clear();
        //inventoryEquipment.Container.Clear();
        //inventoryMain.Container.Clear();
    }

    public void ExitGame()
    {
        DataPersistanceManager.instance.SaveGame();

        Application.Quit();
    }
    #endregion
}
