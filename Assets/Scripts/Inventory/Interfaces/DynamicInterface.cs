using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//placed on inventories
//handles inventory display & interaction (events)
public class DynamicInterface : UserInterface
{
    public GameObject inventoryPrefab;

    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        // Display Toolbar (First 7 elements, index 0-6)
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            //creating slots
            var obj = Instantiate(inventoryPrefab, transform);

            inventory.Container.Items[i].parent = this;

            //setting up events
            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            //filling inventory
            slotsOnInterface.Add(obj, inventory.Container.Items[i]);
        }
    }
}
