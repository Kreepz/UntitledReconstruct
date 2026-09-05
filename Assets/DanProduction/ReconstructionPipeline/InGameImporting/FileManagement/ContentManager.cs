using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class ContentManager
{
    #region Installation
    public static TaskResults InstallLevel(string path)
    {
        TaskResults results = new();
        DirectoryInfo importDirectory = new(path);
        if (!importDirectory.Exists)
        {
            results.SubmitResults(false, "Installation failed");
            results.Errors.Add("Importing content directory not found");
            return results;
        }
        return InstallLevel(new InstallContext(importDirectory));
    }
    public static TaskResults InstallLevel(InstallContext context)
    {
        TaskResults results = new();
        
        TaskResults validationResults = ValidateLevelPackage(context);
        if(!validationResults.Success) return validationResults;

        TaskResults planningResults = PlanLevelPackage(context);
        if (!planningResults.Success) return planningResults;
        
        InstallLevelPackage(context);
        
        results.SubmitResults(true, "Installation complete");
        results.AppendIssues(validationResults);
        results.AppendIssues(planningResults);
        Debug.Log(results.ResultSubmitted);
        return results;
    }

    #region Validation
    static TaskResults ValidateLevelPackage(InstallContext context)
    {
        TaskResults results = new();
        
        //check for file existence
        TaskResults folderStructureResults = ValidatePackageStructure(context);
        if(!folderStructureResults.Success)return folderStructureResults;
        
        //compare metadata to ensure consistency in identity
        TaskResults consistencyResults = ValidatePackageIdentity(context);
        if(!consistencyResults.Success) return consistencyResults;
        
        results.SubmitResults(true, "Validation successful");
        return results;
    }

    static TaskResults ValidatePackageStructure(InstallContext context)
    {
        TaskResults results = new();

        context.Versions = FileServices.GetVersionFolders(context.LevelPackageDirectory);

        if (context.Versions.Length == 0)
        {
            results.SubmitResults(false, "Installation failed");
            results.Errors.Add("No versions found");
            return results;
        }
        
        foreach (DirectoryInfo ver in context.Versions)
        {
            List<string> invalidComponents = new();
            
            FileInfo metadataFile = new(
                Path.Combine(ver.FullName, "metadata.json"));
            if (!metadataFile.Exists)
            {
                invalidComponents.Add("Metadata");
            }
            
            FileInfo thumbnailFile = new(
                Path.Combine(ver.FullName, "thumbnail.png"));
            if (!thumbnailFile.Exists)
            {
                invalidComponents.Add("Thumbnail");
            }
            //do the same for levels

            
            if (invalidComponents.Count > 0)
            {
                results.SubmitResults(false, "Installation failed");
                string errorMessage = $"{ver.Name} is missing the following components: " +
                                      $"{string.Join(", ", invalidComponents)}";
                results.Errors.Add(errorMessage);
                return results;
            }
            
        }
        results.SubmitResults(true, "Structure validation successful");
        return results;
    }

    static TaskResults ValidatePackageIdentity(InstallContext context)
    {
        TaskResults results = new();
        Dictionary<string, List<int>> packageIDs = new();
        int corruptedCount = 0;
        //Load metadata and ensure class compatibility
        foreach (DirectoryInfo ver in context.Versions)
        {
            string metadataPath = Path.Combine(ver.FullName, "metadata.json");
            Debug.Log($"Validating:\n{metadataPath}");
            LevelMetadata metadata = FileServices.GetMetaDataFile(metadataPath);
            if (metadata == null)
            {
                results.SubmitResults(false, "Identity validation failed");
                results.Errors.Add($"{ver.Name} has corrupted metadata");
                corruptedCount++;
                continue;
            }
            
            if(!packageIDs.ContainsKey(metadata.ContentID))
                packageIDs.Add(metadata.ContentID, new List<int>());
            
            packageIDs[metadata.ContentID].Add(metadata.ContentVersion);
            
            if(context.latestMetadata == null || context.latestMetadata.ContentVersion < metadata.ContentVersion)
                context.latestMetadata = metadata;
        }
        if (corruptedCount > 0)
        {
            results.Errors.Add($"A total of {corruptedCount} entries had corrupted data");
            return results;
        }
        
        //check for differences
        if (packageIDs.Count > 1)
        {
            results.SubmitResults(false, "Identity validation failed");
            
            string errorMessage = $"Package contains multiple IDs:\n";
            foreach (KeyValuePair<string, List<int>> id in packageIDs)
            {
                errorMessage += $"{id.Key} -> Versions {string.Join(",", id.Value)}\n";
            }
            
            results.Errors.Add(errorMessage);
            return results;
        }
        context.ContentID =  packageIDs.First().Key;
        results.SubmitResults(true, "Identity validation passed");
        return results;
    }
    #endregion

    #region Planning
    static TaskResults PlanLevelPackage(InstallContext context)
    {
        TaskResults results = new();

        context.InstallationDirectory = new DirectoryInfo(
            Path.Combine(LocalPaths.ContentPath, context.ContentID));

        if (!context.InstallationDirectory.Exists)
        {
            context.UninstalledVersions = context.Versions.ToArray();
            results.SubmitResults(true, "Planning successful,fresh install planned");
            return results;
        }
        
        DirectoryInfo[] installedVersions = FileServices.GetVersionFolders(context.InstallationDirectory);

        context.UninstalledVersions = context.Versions
            .Where(ver => installedVersions.All(installed => installed.Name != ver.Name))
            .ToArray();

        if (context.UninstalledVersions.Length == 0)
        {
            results.SubmitResults(false, "Installation unnecessary");
            results.Warnings.Add("Content is already installed");
            return results;
        }
        results.SubmitResults(true, "Planning successful");
        return results;
    }
    #endregion
    
    #region Writing
    static void InstallLevelPackage(InstallContext context)
    {
        InstallContent(context);
        UpdateCatalogue(context);
    }

    static void InstallContent(InstallContext context)
    {
        context.InstallationDirectory.Create();

        foreach (DirectoryInfo packageVersion in context.UninstalledVersions)
        {
            string versionPath = Path.Combine(
                context.InstallationDirectory.FullName, packageVersion.Name);
            Directory.CreateDirectory(versionPath);
            
            foreach (FileInfo file in packageVersion.GetFiles())
            {
                string destinationPath = Path.Combine(
                    versionPath, file.Name);
                file.CopyTo(destinationPath, true);
            }
        }
    }

    static void UpdateCatalogue(InstallContext context)
    {
        string cataloguePath = Path.Combine(
            LocalPaths.CataloguePath, $"{context.ContentID}.json");
        
        LevelMetadata metadata = FileServices.GetMetaDataFile(cataloguePath);
        if (metadata != null && 
            metadata.ContentVersion >= context.latestMetadata.ContentVersion)
        {
            //Existing catalogue entry is newer or equal
            return;
        } 
        string metadataJson = JsonConvert.SerializeObject(
            context.latestMetadata, Formatting.Indented);
        File.WriteAllText(cataloguePath, metadataJson);
    }
    #endregion
    #endregion

    #region Reading

    public static LevelCollection GetLevelCollection(LevelMetadata metadata)
    {
        DirectoryInfo levelRepo = new(
            Path.Combine(LocalPaths.ContentPath, metadata.ContentID));
        if (!levelRepo.Exists)
        {
            Debug.LogError($"Cannot find the level files for {metadata.ContentID}");
            return null;
        }

        DirectoryInfo versionRepo = new(
            Path.Combine(levelRepo.FullName, $"v{metadata.ContentVersion:D3}"));
        if (!versionRepo.Exists)
        {
            Debug.LogError($"Requested version: {metadata.ContentVersion} does not exist");
            return null;
        }

        string levelPath = Path.Combine(versionRepo.FullName, "level.json");
        return FileServices.GetLevelCollectionFile(levelPath);
    }
    
    public static List<LevelMetadata> GetCatalogue()
    {
        List<LevelMetadata> catalogue = new();
        DirectoryInfo catalogueDirectory = new(LocalPaths.CataloguePath);
        FileInfo[] catalogueEntries = catalogueDirectory.GetFiles("*.json");
        foreach (FileInfo entry in catalogueEntries)
        {
            catalogue.Add(FileServices.GetMetaDataFile(entry.FullName));
        }
        
        return catalogue;
    }

    public static List<int> GetVersions(LevelMetadata metadata)
    {
        List<int> results = new();
        DirectoryInfo levelRepo = new(
            Path.Combine(LocalPaths.ContentPath, metadata.ContentID));

        if (!levelRepo.Exists)
        {
            Debug.LogError($"Cannot find the files for {metadata.ContentID}");
            return null;
        }
        DirectoryInfo[] versions = FileServices.GetVersionFolders(levelRepo);
        
        foreach (DirectoryInfo version in versions)
            results.Add(FileServices.GetVersionNumber(version.Name));
        
        return results;
    }
    
    public static Texture2D GetThumbnail(LevelMetadata metaData)
    {
        return metaData.GetThumbnail();
    }

    public static LevelMetadata GetMetadataVersion(LevelMetadata metaData, int desiredVer)
    {
        DirectoryInfo levelRepo = new(
            Path.Combine(LocalPaths.ContentPath, metaData.ContentID));
        if (!levelRepo.Exists)
        {
            Debug.LogError($"Cannot find the files for {metaData.ContentID}");
            return null;
        }

        DirectoryInfo versionRepo = new(
            Path.Combine(levelRepo.FullName, $"v{desiredVer:D3}"));
        if (!versionRepo.Exists)
        {
            Debug.LogError($"Requested version: {desiredVer} does not exist");
            return null;
        }
        
        string metadataPath = Path.Combine(versionRepo.FullName, "metadata.json");
        return FileServices.GetMetaDataFile(metadataPath);
    }
    #endregion

    #region Management

    public static TaskResults DeleteLevel(LevelMetadata levelData)
    {
        TaskResults results = new();
        string levelID = levelData.ContentID;
        
        //Catalogue
        FileInfo catalogueEntry = new(
            Path.Combine(LocalPaths.CataloguePath, $"{levelID}.json"));
        if (!catalogueEntry.Exists)
        {
            results.SubmitResults(false, "Failed to delete level");
            results.Errors.Add($"Catalogue entry does not exist for: {levelID}");
            return results;
        }
        
        //Content
        DirectoryInfo contentFolder = new(
            Path.Combine(LocalPaths.ContentPath, levelID));
        if (!contentFolder.Exists)
        {
            results.SubmitResults(false, "Failed to delete content");
            results.Errors.Add($"Content folder does not exist for: {levelID}");
            return results;
        }
        
        //Delete
        catalogueEntry.Delete();
        contentFolder.Delete(true);
        
        results.SubmitResults(true, "Deleted content successfully");
        return results;
    }

    public static TaskResults DeleteVersion(LevelMetadata levelData)
    {
        TaskResults results = new();
        string catalogueEntryPath = Path.Combine(LocalPaths.CataloguePath, $"{levelData.ContentID}.json");
        LevelMetadata catalogueEntry = FileServices.GetMetaDataFile(catalogueEntryPath);
        if (catalogueEntry == null)
        {
            results.SubmitResults(false, "Failed to delete version");
            results.Errors.Add($"Catalogue entry does not exist for: {levelData.ContentID}"); 
            return results;
        }
        
        DirectoryInfo versionFolder = new(
            Path.Combine(LocalPaths.ContentPath, levelData.ContentID, $"v{levelData.ContentVersion:D3}"));
        if (!versionFolder.Exists)
        {
            results.SubmitResults(false, "Failed to delete version");
            results.Errors.Add($"Version folder does not exist for: {levelData.ContentID}");
            return results;
        }

        if (catalogueEntry.ContentVersion <= levelData.ContentVersion)
        {
            //find latest version and recreate the catalogue entry
            DirectoryInfo[] versionFolders = FileServices.GetVersionFolders(
                new(Path.Combine(LocalPaths.ContentPath, levelData.ContentID)));

            if (versionFolders.Length <= 1)
            {
                results.SubmitResults(false, "Failed to delete version");
                results.Errors.Add($"Cannot delete versions for level {levelData.ContentID}\n" +
                                    $"Only one version remains, please delete level instead");
                return results;
            }
            DirectoryInfo secondLatestVersion = versionFolders[1];
            string secondLatestMetadataPath = Path.Combine(secondLatestVersion.FullName, $"metadata.json");
            LevelMetadata secondLatestMetadata = FileServices.GetMetaDataFile(secondLatestMetadataPath);
            if (secondLatestMetadata == null)
            {
                results.SubmitResults(false, "Failed to delete version");
                results.Errors.Add($"Could not find new metadata to replace the catalogue\n" +
                               $"Second to last version may have corrupted content {secondLatestVersion.FullName}");
                return results;
            }
            FileInfo newCatalogueEntry = new(secondLatestMetadataPath);
            newCatalogueEntry.CopyTo(catalogueEntryPath, true);
        }
        versionFolder.Delete(true);
        
        results.SubmitResults(true, "Deleted version successfully");
        return results;
    }
    #endregion
}
