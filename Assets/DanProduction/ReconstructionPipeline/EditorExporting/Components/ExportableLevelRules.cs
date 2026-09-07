using UnityEngine;

public abstract class ExportableLevelRules : ScriptableObject
{
    public abstract TaskResults ValidateHierarchy(GameObject rootObject);
}
