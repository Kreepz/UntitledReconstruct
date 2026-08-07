using UnityEngine;

public class ApplicationInitialiser : MonoBehaviour
{
    static bool _initialised = false;
    public void Init()
    {
        if (_initialised) return;
        LibraryInitialiser.Initialise();
    }

    public void DelayedInit()
    {
        if (_initialised) return;
        
        
        
        _initialised = true;
    }
}
