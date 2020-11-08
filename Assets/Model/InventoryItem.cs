using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class InventoryItem {
    [NotNull]
    public int PlayerId { get; set; }

    [NotNull]
    public int ItemId { get; set; }

}
