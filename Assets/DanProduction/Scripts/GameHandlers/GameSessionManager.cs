using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSessionManager
{
    public static LevelMetadata CurrentLevel{ get; private set; }

    public static void StartLevel(LevelMetadata level)
    {
        CurrentLevel = level;
        SceneManager.LoadScene("LevelBuilder");
    }

    public static void ReturnToMainMenu()
    {
        CurrentLevel = null;
        SceneManager.LoadScene("MainMenu");
        GameStateManager.SetState(GameStates.Inactive);
    }
}
