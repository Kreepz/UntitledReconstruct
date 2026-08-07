using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LibraryInitialiser
{
    public static void Initialise()
    {
        EnsureDirectories();
        InstallShippedContent();
    }

    static void EnsureDirectories()
    {
        Directory.CreateDirectory(LocalPaths.CataloguePath);
        Directory.CreateDirectory(LocalPaths.ContentPath);
    }

    static void InstallShippedContent()
    {
        DirectoryInfo[] shippedContent = LocalPaths.ShipExport.GetDirectories();

        foreach (DirectoryInfo level in shippedContent)
        {
            TaskResults results = ContentManager.InstallLevel(new InstallContext(level));
            
            if (results.Warnings.Count > 0)
            {
                string warningList = $"Warnings: {string.Join(", ", results.Warnings)}\n";
                Debug.LogWarning(warningList);
            }
            
            if (!results.Success)
            {
                Debug.LogError($"Ran into trouble installing level {level.Name}");
                if (results.Errors.Count > 0)
                {
                    string errorList = $"Errors: {string.Join(", ", results.Errors)}\n";
                    Debug.LogError(errorList);
                }
            }
        }
    }
}
