using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReloadableObject
{
    public ReloadableObject(GameObject gameObject)
    {
        GameObject = gameObject;
        _behaviourComponents = gameObject.GetComponents<ReconstructableBehaviour>().ToList();
    }
    
    GameObject GameObject{get;}
    readonly List<ReconstructableBehaviour> _behaviourComponents;
    public Vector3 Position{get;set;}
    public Quaternion Rotation{get;set;}
    public Vector3 Scale{get;set;}


    public void ResetTransform()
    {
        GameObject.transform.position = Position;
        GameObject.transform.rotation = Rotation;
        GameObject.transform.localScale = Scale;
    }
    public void Reload()
    {
        foreach (ReconstructableBehaviour behaviour in _behaviourComponents)
        {
            behaviour.OnLevelLoaded();
        }
    }

    public void Restart()
    {
        foreach (ReconstructableBehaviour behaviour in _behaviourComponents)
        {
            behaviour.OnLevelStart();
        }
    }
}
