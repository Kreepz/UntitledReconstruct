using UnityEngine;

[System.Serializable]
public class ReconstructableFloat: ReconstructableParameter<float>
{
    public override string ID { get; set; }
    [SerializeField] float  floatValue;
    public override float Value => floatValue;

    public void SetValue(float? value)
    {
        if(value.HasValue)
            floatValue = value.Value;
        else
            Debug.LogError($"Tried to set {ID} to a null value");
    }
}
