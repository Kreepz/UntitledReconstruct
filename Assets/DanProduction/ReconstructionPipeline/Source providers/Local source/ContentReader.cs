using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class ContentReader
{
    public static async Task<List<LevelMetadata>> LoadCatalogue(string path)
    {
        string[] metadataFiles = Directory.GetFiles(
            path,
            "*.json",
            SearchOption.TopDirectoryOnly);
        
        List<LevelMetadata> results = new();

        foreach (string file in metadataFiles)
        {
            try
            {
                string json = await File.ReadAllTextAsync(file);

                LevelMetadata levelEntry = JsonUtility.FromJson<LevelMetadata>(json);

                if (levelEntry != null)
                    results.Add(levelEntry);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }

        return results;
        
    }
}