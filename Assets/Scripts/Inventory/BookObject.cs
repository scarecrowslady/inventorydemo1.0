using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Book Object", menuName = "Inventory System/Items/Book")]
public class BookObject : ItemObject
{
    [TextArea(5, 5)]
    public string title;

    [TextArea(10, 10)]
    public string content;

    public void Awake()
    {
        //type = ItemType.Book;
    }
}
