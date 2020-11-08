using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Item {
    [PrimaryKey, AutoIncrement]
    public int ItemId { get; set; }
    [NotNull]
    public string Description { get; set; }
   
    public int Effect { get; set; }  // will link to change in conditions for the player in scenes?, situation programming
}
