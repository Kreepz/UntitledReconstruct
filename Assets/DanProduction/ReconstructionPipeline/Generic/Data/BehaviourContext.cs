
#nullable enable
using System.Collections.Generic;
using UnityEngine;

public class BehaviourContext
{
    public string BehaviourID { get; set; }
    public List<ReconstructableParameter> Parameters { get; set; } = new();


    public bool TryGetParameterValue<T>(string id, out T? value)
    {
        ReconstructableParameter parameter = Parameters.Find(p => p.ID == id);

        if (parameter is ReconstructableParameter<T> typedParameter)
        {
            value = typedParameter.Value;
            return true;
        }
        
        value = default;
        return false;
    }
    
}
