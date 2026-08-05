using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LocalProvider : IContentProvider
{
    public async Task<List<LevelMetadata>> GetLevelCatalogueAsync()
    {
        List<LevelMetadata> catalogue = new();
        try
        {
            catalogue = await ContentReader.LoadCatalogue(LocalPaths.LibraryPath);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
        return catalogue;
    }
}
