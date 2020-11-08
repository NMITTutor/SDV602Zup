using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Story  {
    [PrimaryKey]
    public string StoryName { get; set; }
    public string Description{ get; set; }
    public int FirstSceneID { get; set; }
}
