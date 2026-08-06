using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class LocalExporter
{
    static readonly ExportDisplay ExportDisplay = new();
    
    public static void ExportLevel(ExportableLevelRoot root, ExportSettings settings, DirectoryInfo depositDirectory)
    {
        ExportContext context = new(root, settings, depositDirectory);
        if (!ValidateExport(context))
        {
            Debug.LogError($"Validation failed, check errors");
            ExportDisplay.CloseProgressionBar();
            return;
        }
        Debug.Log("Validation succeeded");
        if (!ResolveExport(context))
        {
            Debug.LogError($"Resolution failed, check errors");
            ExportDisplay.CloseProgressionBar();
            return;
        }
        Debug.Log("Resolution succeeded");
        
        CompileExport(context);
        Debug.Log("Compilation succeeded");
        
        DeployExport(context);
        Debug.Log("Export complete");
        ExportDisplay.CloseProgressionBar();
    }

    #region Validation functions
    static bool ValidateExport(ExportContext context)
    {
        //start validating metadata format
        ExportDisplay.StartStage(ExportStage.Validating);
        List<bool> results = new();
        results.Add(context.RootComponent.AuthoredMetadata.ValidateData());
        
        ExportDisplay.UpdateTask(ValidatingStage.ValidatingHierarchy);
        results.Add(context.RootComponent.ValidateHierarchy());
        
        bool success = !results.Contains(false);
        return success;
    }
    
    #endregion

    #region Resolution functions

    static bool ResolveExport(ExportContext context)
    {
        ExportDisplay.StartStage(ExportStage.Resolving);
        List<bool> results = new();
        results.Add(ResolveLevelDirectory(context));
        
        ExportDisplay.UpdateTask(ResolutionStage.ResolvingVersion);
        results.Add(ResolveVersion(context));
        
        bool success = !results.Contains(false);
        return success;
    }
    
    static bool ResolveLevelDirectory(ExportContext context)
    {
        if(!context.DepositDirectory.Exists) 
            context.DepositDirectory.Create();
        
        //setup identifiers
        string idSnippet = context.RootComponent.AuthoredMetadata.LevelId[..10];
        string folderName = $"{context.RootComponent.AuthoredMetadata.LevelName}_{idSnippet}";
        
        //check possible entry matches
        DirectoryInfo[] matches = context.DepositDirectory.GetDirectories()
            .Where(folder => folder.Name.EndsWith($"_{idSnippet}"))
            .ToArray();
        
        //if no matches exist create a new one and proceed
        if (matches.Length == 0)
        {
            context.LevelDirectory = context.DepositDirectory.CreateSubdirectory(folderName);
            return true;
        }
        
        //check through versions for verification
        DirectoryInfo? matchDirectory = null;
        foreach (DirectoryInfo candidate in matches)
        {
            if (matchDirectory != null) break;
            DirectoryInfo[] versions = candidate.GetDirectories()
                .Where(folder => FileServices.GetVersionNumber(folder.Name) >= 0)
                .OrderByDescending(folder => FileServices.GetVersionNumber(folder.Name))
                .ToArray();
            
            //if no version folders exist, cancel function and return an error
            if (versions.Length == 0)
            {
                Debug.LogError($"{candidate.FullName} is corrupted," +
                               $"missing version folders, ensure proper name formatting" +
                               $"e.g. V001");
                return false;
            }
            
            //compare the versioned folders inside the level repository
            LevelMetadata discoveredMetadata = null;
            foreach (DirectoryInfo version in versions)
            {
                //check through contents inside the versioned folders for metadata
                //for ID comparisons, keep searching until a valid comparison can be made
                string metadataPath = Path.Combine(version.FullName, "metadata.json");
                
                if (!File.Exists(metadataPath))
                {
                    Debug.LogError($"metadata is missing from {version.FullName}");
                    continue;
                }
                
                string json = File.ReadAllText(metadataPath);
                discoveredMetadata = JsonUtility.FromJson<LevelMetadata>(json);

                if (discoveredMetadata == null)
                {
                    Debug.LogError($"Invalid metadata in {version.FullName}");
                    continue;
                }
                
                //if a valid entry is found exit out of the loop.
                //Set variables only if the correct match is found
                if (discoveredMetadata.ContentID == context.Metadata.ContentID)
                {
                    matchDirectory = candidate;
                }
                break;
            }
            
            //if no comparisons could be executed in the previous search loop
            //exit the function and return a false result.
            if (discoveredMetadata == null)
            {
                return false;
            } 
        }
        
        //resolve results after validation
        if (matchDirectory != null)
        {
            if (matchDirectory.Name != folderName)
            {
                string newPath = Path.Combine(context.DepositDirectory.FullName, folderName);
                
                //extremely unlikely case, but just in case
                if (Directory.Exists(newPath))
                {
                    Debug.LogError($"Attempting to move {matchDirectory.FullName} to" +
                                   $"\n {newPath}" +
                                   $"\n however, the new directory already exists");
                    return false;
                }
                
                //otherwise apply normally
                matchDirectory.MoveTo(newPath);
                context.LevelDirectory = new DirectoryInfo(newPath);
            }
            else
            {
                context.LevelDirectory = matchDirectory;
            }
        }
        else
        {
            context.LevelDirectory = 
                context.DepositDirectory.CreateSubdirectory(folderName);
        }
        return true;
    }

    static bool ResolveVersion(ExportContext context)
    {
        DirectoryInfo latestFolder = context.LevelDirectory.GetDirectories()
            .Where(folder => FileServices.GetVersionNumber(folder.Name) >= 0)
            .OrderByDescending(folder => FileServices.GetVersionNumber(folder.Name))
            .FirstOrDefault();
        int latestVer = latestFolder == null
            ? 0: FileServices.GetVersionNumber(latestFolder.Name);

        if (context.Settings.AutomaticVersioning)
        {
            int resolvedVersion = 1;
            if (latestVer > 0)
                resolvedVersion = latestVer + 1;
            context.Metadata.SetContentVersion(resolvedVersion);
        }
        else
        {
            if (latestVer >= context.RootComponent.AuthoredMetadata.ContentVer)
            {
                Debug.LogError(
                    $"Entered version :{context.Metadata.ContentVersion} already exists " +
                    $"or is older than the latest version :{latestVer}");
                return false;
            }
        }
        context.FinalDirectory = 
            context.LevelDirectory.CreateSubdirectory($"v{context.RootComponent.AuthoredMetadata.ContentVer:D3}");
        return true;
    }
    
    #endregion
    
    #region Compiler functions
    static void CompileExport(ExportContext context)
    {
        ExportDisplay.StartStage(ExportStage.Compiling);
        context.Metadata = context.RootComponent.GetLevelMetadata();
        
        ExportDisplay.UpdateTask(CompilationStage.CompilingThumbnail);
        context.ThumbnailImage = ImageCompiler.ToPng(context.RootComponent.AuthoredMetadata.Thumbnail);
        
        ExportDisplay.UpdateTask(CompilationStage.CompilingLevel);
        //call root to compile level and extract into the portable format
    }

    #endregion

    #region Deployment functions
    static void DeployExport(ExportContext context)
    {
        ExportDisplay.StartStage(ExportStage.Deploying);
        DeployMetadata(context);
        
        ExportDisplay.UpdateTask(DeployStage.DeployingThumbnail);
        DeployThumbnail(context);
    }

    static void DeployMetadata(ExportContext context)
    {
        string json = JsonConvert.SerializeObject
            (context.Metadata, Formatting.Indented);
        string metadataPath = Path.Combine
            (context.FinalDirectory.FullName, $"metadata.json");
        File.WriteAllText(metadataPath, json);
    }

    static void DeployThumbnail(ExportContext context)
    {
        bool shipping = context.DepositDirectory.FullName == LocalPaths.ShipExport.FullName;
        string thumbnailPath = Path.Combine
            (context.FinalDirectory.FullName, $"thumbnail.png");
        File.WriteAllBytes(thumbnailPath, context.ThumbnailImage);

        if (!shipping)
        {
            AssetDatabase.Refresh();
            string assetPath = LocalPaths.GetAssetPath(thumbnailPath);
            TextureImporter importer = 
                AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (!importer)
            {
                throw new Exception($"Failed to retrieve importer for {assetPath}");
            }
            importer.textureType = TextureImporterType.Default;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }
    #endregion
    
}
