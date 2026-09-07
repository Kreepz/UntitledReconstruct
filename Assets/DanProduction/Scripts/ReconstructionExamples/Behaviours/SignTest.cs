using TMPro;
using UnityEngine;

public class SignTest : ReconstructableBehaviour
{
    [SerializeField] TextMeshProUGUI signText;
    public override string BehaviourID => "Sign";
    

    [SerializeField] ReconstructableString signContent = new()
    {
        ID =  "SignContent"
    };
    
    public override void OnLevelLoaded()
    {
        signText.text = signContent.Value;
    }

    public override void OnLevelStart()
    {
        
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
