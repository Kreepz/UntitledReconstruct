using System.Collections.Generic;
using UnityEngine;

public class LevelCollection
{
    public List<LevelObject> Children { get; set; }

    public LevelCollection(GameObject rootObject)
    {
        Children = new List<LevelObject>();
        foreach (Transform child in rootObject.transform)
        {
            LevelObject immediateChild = GameObjectConverter.ConvertToLevelObject(child.gameObject);
            if(immediateChild != null)
                Children.Add(immediateChild);
        }
    }
}
