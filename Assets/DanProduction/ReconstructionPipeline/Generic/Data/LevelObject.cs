using System.Collections.Generic;
using UnityEngine;

public class LevelObject
{
    public string AssetKey { get; set; }
    
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public List<LevelObject> Children { get; set; }
    //Monobehaviour context
}

public enum LevelObjectType
{
    GroupingNode,
    ExtractableObject,
    Ignore
}