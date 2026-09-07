using System;
using UnityEngine;

public class LauncherTest : ReconstructableBehaviour
{
    public override string BehaviourID => "PlayerLauncher";

    [SerializeField] ReconstructableFloat jumpStrength = new()
    {
        ID = "jumpStrength"
    };

    public override void OnLevelLoaded()
    {
    }

    public override void OnLevelStart()
    { }
    
    public override BehaviourContext CompileContext()
    {
        BehaviourContext ctx = new()
        {
            BehaviourID = BehaviourID
        };
        ctx.Parameters.Add(jumpStrength);
        return ctx;
    }
    public override void ImportContext(BehaviourContext ctx)
    {
        if (BehaviourID != ctx.BehaviourID)
        {
            Debug.LogError($"{gameObject.name} tried to initialise with incorrect context," +
                           $"component : {BehaviourID}, with: {ctx.BehaviourID}");
            return;
        }
        
        if(ctx.TryGetParameterValue(jumpStrength.ID, out float jumpStrengthValue))
            jumpStrength.SetValue(jumpStrengthValue);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            other.GetComponent<Rigidbody>().linearVelocity = Vector3.up * jumpStrength.Value;
    }
}
