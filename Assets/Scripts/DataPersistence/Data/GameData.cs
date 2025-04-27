using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int deathCount;

    public PlayerInventoryData_Main playerinventoryData;
    public PlayerInventoryData_Equipment equipmentData;
    public PlayerInventoryData_Toolbar toolbarData;

    // default values on initial load
    public GameData()
    {
        deathCount = 0;

        playerinventoryData = new PlayerInventoryData_Main();
        equipmentData = new PlayerInventoryData_Equipment();
        toolbarData = new PlayerInventoryData_Toolbar();
    }
}
