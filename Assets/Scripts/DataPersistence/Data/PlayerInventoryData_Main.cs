using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventoryData_Main
{
    public List<InventorySlot> Container = new List<InventorySlot>();

    public PlayerInventoryData_Main()
    {
        Container.Clear();
    }
}
