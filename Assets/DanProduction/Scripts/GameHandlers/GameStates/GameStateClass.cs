using UnityEngine;

public abstract class GameStateClass : MonoBehaviour
{
    public abstract void Enter();
    public abstract void StateUpdate();
    public abstract void Exit();
}
