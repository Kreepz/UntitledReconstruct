using UnityEngine;
[System.Serializable]
public class ReconstructableString: ReconstructableParameter<string>
{
    public override string ID { get; set; }

    [SerializeField] string stringValue;
    public override string Value => stringValue;

    public void SetStringValue(string value)
    {
        stringValue = value;
    }
}
