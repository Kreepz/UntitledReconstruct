using System;
using UnityEngine;

public static class GameStateManager
{
    //States
    static GameStates _currentState;
    public static GameStates CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState == value) return;
            
            GameStates previousState = _currentState;
            _currentState = value;
            
            HandleStateChange(previousState, _currentState);
        }
    }
    
    
    static event Action OnLoadState;
    static event Action OnLoadStateExit;

    static event Action OnRunState;
    static event Action OnRunStateExit;
    
    public static bool GamePaused => _currentState == GameStates.Paused;
    static event Action OnPauseState;
    static event Action OnPauseStateExit;
    
    public static void SetupState(GameStates state, Action enterCallback, Action exitCallback = null)
    {
        switch (state)
        {
            case GameStates.LoadingLevel:
                OnLoadState = enterCallback;
                OnLoadStateExit = exitCallback;
                break;
            case GameStates.Running:
                OnRunState = enterCallback;
                OnRunStateExit = exitCallback;
                break;
            case GameStates.Paused:
                OnPauseState = enterCallback;
                OnPauseStateExit = exitCallback;
                break;
        }
    }

    public static void ClearState(GameStates state)
    {
        switch (state)
        {
            case  GameStates.LoadingLevel:
                OnLoadState = null;
                OnLoadStateExit = null;
                break;
            case  GameStates.Running:
                OnRunState = null;
                OnRunStateExit = null;
                break;
            case  GameStates.Paused:
                OnPauseState = null;
                OnPauseStateExit = null;
                break;
        }
    } 
    
    static void HandleStateChange(GameStates previousState, GameStates newState)
    {
        switch (previousState)
        {
            case  GameStates.LoadingLevel:
                OnLoadStateExit?.Invoke();
                break;
            case  GameStates.Running:
                OnRunStateExit?.Invoke();
                break;
            case  GameStates.Paused:
                OnPauseStateExit?.Invoke();
                break;
        }
        switch (newState)
        {
            case  GameStates.LoadingLevel:
                OnLoadState?.Invoke();
                break;
            case  GameStates.Running:
                OnRunState?.Invoke();
                break;
            case  GameStates.Paused:
                OnPauseState?.Invoke();
                break;
        }
    }
    
    public static void SetState(GameStates state)
    {
        CurrentState = state;
    }
}
