using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//create databases
[CreateAssetMenu(fileName = "New Database", menuName = "Inventory System/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();

    public void OnAfterDeserialize()
    {
        for(int i = 0; i < Items.Length; i++)
        {
            Items[i].data.Id = i;
            GetItem.Add(i, Items[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        GetItem = new Dictionary<int, ItemObject>();
    }
}
