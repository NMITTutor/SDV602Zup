using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

/*
 * Issue with multiple primary keys - how to do that
 * Issue with foreign key - 
 * Using NotNull as a backstop.
 */
public class SceneDirection{
    [NotNull]
    public int FromSceneId { get; set; }
    public int ToSceneId { get; set; }
    [NotNull]
    public string Label { get; set; }

}