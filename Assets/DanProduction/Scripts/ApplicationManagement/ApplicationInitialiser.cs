using System;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;

public class ApplicationInitialiser : MonoBehaviour
{
    static bool _initialised = false;
    public void Init()
    {
        if (_initialised) return;
        string downloadsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads");

        FileBrowser.AddQuickLink(
            "Downloads",
            downloadsPath);
        
        LibraryInitialiser.Initialise();
    }

    public void DelayedInit()
    {
        if (_initialised) return;
        
        _initialised = true;
    }
}
