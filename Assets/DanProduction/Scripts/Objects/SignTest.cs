using UnityEngine;

public class SignTest : ReconstructableBehaviour
{
    public override string BehaviourID => "Sign";

    [SerializeField] ReconstructableString signContent = new()
    {
        ID =  "SignContent"
    };
    
    public override void OnLevelLoaded()
    {
    }

    public override void OnLevelStart()
    {
        Debug.Log($"Game starting, displaying message: {signContent.Value}");
    }

    public override BehaviourContext CompileContext()
    {
        BehaviourContext ctx = new()
        {
            BehaviourID =  BehaviourID,
        };
        ctx.Parameters.Add(signContent);
        
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
        
        if(ctx.TryGetParameterValue(signContent.ID, out string signValue))
            signContent.SetStringValue(signValue);
        
    }
}
