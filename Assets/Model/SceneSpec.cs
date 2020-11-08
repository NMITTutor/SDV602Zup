using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpec {
    public string Description;
    public Dictionary<string,SceneSpec> ToScenes;
    public Item[] Items;	
}
