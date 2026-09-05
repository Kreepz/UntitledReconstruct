using UnityEngine;

public abstract class ExtractableLevelRules : ScriptableObject
{
    public abstract TaskResults ValidateHierarchy(GameObject rootObject);
}
