using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class LocalExporter
{
    public static void ExportLevel(LevelMetadata exportingMetadata, Texture2D thumbnail)
    {
        EnsureDepositDirectory();
        
        //prepare naming
        string guidSnippet = exportingMetadata.ContentID[..10];
        string folderName = $"{exportingMetadata.LevelName}_{guidSnippet}";
        
        //search existing directory
        DirectoryInfo directory = new DirectoryInfo(LocalPaths.ExportPath);
        DirectoryInfo existingFolder = directory.GetDirectories().FirstOrDefault(folder => folder.Name.EndsWith($"_{guidSnippet}"));

        bool directoryExists = false;
        //Validate directory
        if (existingFolder != null)
        {
            directoryExists = true;
            DirectoryInfo latestFolder = existingFolder.GetDirectories().OrderByDescending(folder => GetVersionNumber(folder.Name)).FirstOrDefault();
            if (latestFolder != null)
            {
                //assuring id match
                string metadataPath = Path.Combine(latestFolder.FullName, "metadata.json");
                if (!File.Exists(metadataPath))
                {
                    Debug.LogError($"metadata.json missing from {latestFolder.FullName}");
                    return;
                }
                string json = File.ReadAllText(metadataPath);
                LevelMetadata latestMetadata = JsonUtility.FromJson<LevelMetadata>(json);
                directoryExists = exportingMetadata.ContentID == latestMetadata.ContentID;

                if (directoryExists)
                {
                    //versioning format validation
                    int latestVer = GetVersionNumber(latestFolder.Name);
                    if (latestVer == exportingMetadata.ContentVersion)
                    {
                        Debug.LogError("Version already exists, please increment");
                        return;
                    }
                    if (latestVer > exportingMetadata.ContentVersion)
                    {
                        Debug.Log("Exporting version is lower than latest, please increment");
                        return;
                    }
                }
            }
        }
        
        string newPath = Path.Combine(LocalPaths.ExportPath, folderName);
        
        //If directory doesn't exist, create one
        if (!directoryExists)
        {
            Directory.CreateDirectory(newPath);
        }
        
        //if directory does exist but the name is different, update the name
        else if(existingFolder.Name != folderName)
        {
            Directory.Move(existingFolder.FullName, newPath);
            existingFolder = new DirectoryInfo(newPath);
        }
    }
    
    public static void ShipLevel(LevelMetadata levelMetadata, Texture2D thumbnail)
    {
        
    }

    //helper functions

    static DirectoryInfo CheckExistingEntries(string folderName)
    {
        
        return null;
    }
    static void EnsureDepositDirectory()
    {
        if (!Directory.Exists(LocalPaths.ExportPath))
        {
            Directory.CreateDirectory(LocalPaths.ExportPath);
            AssetDatabase.Refresh();
        }
    }
    static int GetVersionNumber(string folderName)
    {
        //expected format : v001
        if (folderName.StartsWith("v") &&
            int.TryParse(folderName.Substring(1), out int version))
        {
            return version;
        }
        return -1;
    }
    
}
