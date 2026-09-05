using UnityEngine;

[System.Serializable]
public class ReconstructableInt : ReconstructableParameter<int>

{
    public override string ID { get; set; }
    [SerializeField] int intValue; 
    public override int Value => intValue;

    public void SetValue(int? value)
    {
        if(value.HasValue)
            intValue = value.Value;
        else
            Debug.LogError($"Tried to set {ID} to a null value");
    }
}
