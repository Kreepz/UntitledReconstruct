using UnityEngine;

public class GamePlayBootstrapper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameStateManager.SetState(GameStates.LoadingLevel);
        Debug.Log("Calling state manager to load ");
    }
}
