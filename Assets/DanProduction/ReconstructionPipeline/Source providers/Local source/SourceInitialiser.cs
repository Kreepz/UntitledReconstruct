using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SourceInitialiser
{
    public void Initialise()
    {
        EnsureDirectories();
        BuildShippedContent();
    }

    void EnsureDirectories()
    {
        if (!Directory.Exists(LocalPaths.LibraryPath))
            Directory.CreateDirectory(LocalPaths.LibraryPath);
        if (!Directory.Exists(LocalPaths.ContentPath))
            Directory.CreateDirectory(LocalPaths.ContentPath);
    }

    public async void BuildShippedContent(bool forceOverWrite = false)
    {
        List<LevelMetadata> shippedLibrary = await ContentReader.LoadCatalogue(LocalPaths.ShippedLibraryPath);
        List<LevelMetadata> library = await ContentReader.LoadCatalogue(LocalPaths.LibraryPath);
        if (forceOverWrite)
        {
            
        }
    }
}
