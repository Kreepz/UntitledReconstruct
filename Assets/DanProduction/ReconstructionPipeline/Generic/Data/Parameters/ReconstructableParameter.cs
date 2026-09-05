using UnityEngine;

public abstract class ReconstructableParameter
{
    public abstract string ID { get; set; }
}
[System.Serializable]
public abstract class ReconstructableParameter<T> : ReconstructableParameter
{
    public abstract T Value { get;}
}
