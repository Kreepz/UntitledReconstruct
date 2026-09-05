using UnityEngine;

public abstract class ReconstructableBehaviour : MonoBehaviour
{
    public abstract string BehaviourID { get;}
    public abstract void OnLevelLoaded();
    public abstract void OnLevelStart();
    
    
    public abstract BehaviourContext CompileContext();
    public abstract void ImportContext(BehaviourContext ctx);
}
