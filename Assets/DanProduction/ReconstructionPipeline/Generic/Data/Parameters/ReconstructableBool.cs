using UnityEngine;

[System.Serializable]
public class ReconstructableBool : ReconstructableParameter<bool>
{
    public override string ID { get; set; }
    [SerializeField] bool  boolValue;
    public override bool Value => boolValue;

    public void SetBoolValue(bool? value)
    {
        if (value.HasValue)
            boolValue = value.Value;
        else
            Debug.LogError($"Tried to set {ID} to a null value");
    }
}
