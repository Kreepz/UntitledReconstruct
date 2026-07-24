using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class LocalExporter
{
    public static void ExportLevel(LevelMetadata exportingMetadata, Texture2D thumbnail)
    {
        DirectoryInfo repo = EnsureDepositDirectory();
        DirectoryInfo targetDirectory = ResolveExportsDirectory(repo, exportingMetadata.LevelName, exportingMetadata.ContentID);
        
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

    static DirectoryInfo ResolveExportsDirectory(DirectoryInfo repo, string contentName, string id)
    {
        if (repo == null)
        {
            Debug.LogError("Directory of repo is null, please ensure it exists.");
            return null;
        }
        DirectoryInfo result = null;
        string idSnippet = id[..10];
        string folderName = $"{contentName}_{idSnippet}";
        string targetPath = Path.Combine(LocalPaths.ExportPath, folderName);

        
        //check for possible entry matches
        DirectoryInfo[] matches = repo.GetDirectories()
            .Where(folder => folder.Name.EndsWith($"_{idSnippet}"))
            .ToArray();

        //compare all
        bool matchFound = false;
        foreach (DirectoryInfo candidate in matches)
        {
            if (matchFound) break;
            bool isValidStructure = false;
            
            DirectoryInfo[] versions = candidate.GetDirectories()
                .Where(folder => GetVersionNumber(folder.Name) >= 0)
                .OrderByDescending(folder => GetVersionNumber(folder.Name))
                .ToArray();

            if (versions.Length > 0)
            {
                isValidStructure = true;
                foreach (DirectoryInfo version in versions)
                {
                    string metadataPath = Path.Combine(version.FullName, "metadata.json");
                    
                    if (!File.Exists(metadataPath))
                    {
                        Debug.LogError($"metadata.json missing from {version.FullName}");
                        isValidStructure = false;
                        continue;
                    }
                    
                    isValidStructure = true;
                    string json = File.ReadAllText(metadataPath);
                    LevelMetadata metaData = JsonUtility.FromJson<LevelMetadata>(json);
                    
                    if (metaData == null)
                    {
                        Debug.LogError($"Invalid metadata.json in {version.FullName}");
                        isValidStructure = false;
                        continue;
                    }
                    
                    if (metaData.ContentID != id) continue;
                    
                    result = candidate;
                    matchFound = true;
                    break;

                }
            }
            else Debug.LogError($"{candidate.FullName} is missing version folders, " +
                                $"ensure proper name formatting e.g. V001");
            
            if (!isValidStructure)
            {
                Debug.LogError($"{candidate.FullName} is corrupted, please check previous errors and fix");
                return null;
            }
        }
        
        //if not, create a new one and return
        if (result == null)
        {
            Directory.CreateDirectory(targetPath);
        }
        else if (result.FullName != targetPath)
        {
            Directory.Move(result.FullName, targetPath);
        }
        result = new DirectoryInfo(targetPath);
        return result;
    }
    
    static DirectoryInfo EnsureDepositDirectory()
    {
        if (!Directory.Exists(LocalPaths.ExportPath))
        {
            Directory.CreateDirectory(LocalPaths.ExportPath);
            AssetDatabase.Refresh();
        }
        return new DirectoryInfo(LocalPaths.ExportPath);
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
