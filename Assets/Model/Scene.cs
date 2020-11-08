using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Scene  {
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    [NotNull]
    public string StoryName { get; set; }
    public string Description { get; set; }

}
