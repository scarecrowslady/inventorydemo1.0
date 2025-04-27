using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PlayerInventoryData_Toolbar
{
    public List<InventorySlot> Container = new List<InventorySlot>();

    public PlayerInventoryData_Toolbar()
    {
        Container.Clear();
    }
}
