#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public static class LocalExporter
{
    static readonly ExportDisplay ExportDisplay = new();
    
    public static void ExportLevel(ExportableLevelRoot root, ExportSettings settings, DirectoryInfo depositDirectory)
    {
        ExportContext context = new(root, settings, depositDirectory);
        
        TaskResults validationResults = ValidateExport(context);
        DebugDisplayTaskResults(validationResults);
        if (!validationResults.Success)
        { 
            ExportDisplay.CloseProgressionBar();
            return;
        }
        
        TaskResults resolutionResults = ResolveExport(context);
        DebugDisplayTaskResults(resolutionResults);
        if (!resolutionResults.Success)
        {
            ExportDisplay.CloseProgressionBar();
            return;
        }
        
        CompileExport(context);
        Debug.Log("Compilation succeeded");
        
        DeployExport(context);
        Debug.Log("Export complete");
        ExportDisplay.CloseProgressionBar();
        
        SignOffExport(context.FinalDirectory.FullName);
    }

    #region Validation functions
    static TaskResults ValidateExport(ExportContext context)
    {
        //start validating metadata format
        ExportDisplay.StartStage(ExportStage.Validating);
        TaskResults dataValidationResults = 
            context.RootComponent.AuthoredMetadata.ValidateData();
        if(!dataValidationResults.Success)
            return dataValidationResults;
        
        //validate hierarchy
        ExportDisplay.UpdateTask(ValidatingStage.ValidatingHierarchy);
        TaskResults hierarchyValidationResults =
            context.RootComponent.ValidateHierarchy();
        if (!hierarchyValidationResults.Success)
            return hierarchyValidationResults;

        //build unified final report
        TaskResults validationResults = new();
        validationResults.AppendIssues(dataValidationResults);
        validationResults.AppendIssues(hierarchyValidationResults);
        validationResults.SubmitResults(true, "Validation passed");
        return validationResults;
    }
    
    #endregion

    #region Resolution functions

    static TaskResults ResolveExport(ExportContext context)
    {
        ExportDisplay.StartStage(ExportStage.Resolving);
        TaskResults directoryResults = ResolveLevelDirectory(context);
        if (!directoryResults.Success) 
            return directoryResults;
        
        ExportDisplay.UpdateTask(ResolutionStage.ResolvingVersion);
        TaskResults versioningResults = ResolveVersion(context);
        if (!versioningResults.Success)
            return versioningResults;
        
        ExportDisplay.UpdateTask(ResolutionStage.ResolvingImageCompiler);
        ResolveImageCompiler(context);
        
        TaskResults results = new();
        results.AppendIssues(directoryResults);
        results.AppendIssues(versioningResults);
        results.SubmitResults(true, "Resolution successful");
        return results;
    }
    
    static TaskResults ResolveLevelDirectory(ExportContext context)
    {
        TaskResults results = new();
        if(!context.DepositDirectory.Exists) 
            context.DepositDirectory.Create();
        
        //setup identifiers
        string idSnippet = context.RootComponent.AuthoredMetadata.LevelId[..10];
        string safeName = FileServices.GetSafeFileName(context.RootComponent.AuthoredMetadata.LevelName);
        string folderName = $"{safeName}_{idSnippet}";
        
        //check possible entry matches
        DirectoryInfo[] matches = context.DepositDirectory.GetDirectories()
            .Where(folder => folder.Name.EndsWith($"_{idSnippet}"))
            .ToArray();
        
        //if no matches exist create a new one and proceed
        if (matches.Length == 0)
        {
            context.LevelDirectory = context.DepositDirectory.CreateSubdirectory(folderName);
            results.SubmitResults(true, "New content detected, created new directory");
            return results;
        }
        
        //check through versions for verification
        DirectoryInfo? matchDirectory = null;
        foreach (DirectoryInfo candidate in matches)
        {
            if (matchDirectory != null) break;
            
            DirectoryInfo[] versions = FileServices.GetVersionFolders(candidate);
                /*
                candidate.GetDirectories()
                .Where(folder => FileServices.GetVersionNumber(folder.Name) >= 0)
                .OrderByDescending(folder => FileServices.GetVersionNumber(folder.Name))
                .ToArray();
                */
                
            //if no version folders exist, cancel function and return an error
            if (versions.Length == 0)
            {
                results.SubmitResults(false, "Corrupted directory detected");
                results.Errors.Add($"{candidate.FullName} is missing version folders," +
                                   $"ensure proper formatting, e.g. V001");
                return results;
            }
            
            //compare the versioned folders inside the level repository
            LevelMetadata discoveredMetadata = null;
            foreach (DirectoryInfo version in versions)
            {
                //check through contents inside the versioned folders for metadata
                //for ID comparisons, keep searching until a valid comparison can be made
                string metadataPath = Path.Combine(version.FullName, "metadata.json");
                discoveredMetadata = FileServices.GetMetaDataFile(metadataPath);
                
                //if a valid entry is found exit out of the loop.
                //Set variables only if the correct match is found
                if (discoveredMetadata.ContentID == context.RootComponent.AuthoredMetadata.LevelId)
                {
                    matchDirectory = candidate;
                }
                break;
            }
            
            //if no comparisons could be executed in the previous search loop
            //exit the function and return a false result.
            if (discoveredMetadata == null)
            {
                results.SubmitResults(false, "Resolution failed, attempted comparison could not be executed");
                results.Errors.Add($"No valid metadata was found in {candidate.FullName}, directory may be corrupted");
                return results;
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
                    results.SubmitResults(false, "Resolution unable to complete");
                    results.Errors.Add($"Attempting to move {matchDirectory.FullName} to" +
                                   $"\n {newPath}" +
                                   $"\n however, that directory already exists");
                    return results;
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
        results.SubmitResults(true, "Resolution completed");
        return results;
    }

    static TaskResults ResolveVersion(ExportContext context)
    {
        TaskResults results = new();
        DirectoryInfo latestFolder = FileServices.GetVersionFolders(context.LevelDirectory)
            .FirstOrDefault();
        
        int latestVer = latestFolder == null
            ? 0: FileServices.GetVersionNumber(latestFolder.Name);

        if (context.Settings.AutomaticVersioning)
        {
            int resolvedVersion = 1;
            if (latestVer > 0)
                resolvedVersion = latestVer + 1;
            context.RootComponent.AuthoredMetadata.ContentVer = resolvedVersion;
        }
        else
        {
            if (latestVer >= context.RootComponent.AuthoredMetadata.ContentVer)
            {
                results.SubmitResults(false, "Version resolution failed");
                results.Errors.Add($"Entered version : {context.RootComponent.AuthoredMetadata.ContentVer} already exists " +
                                   $"or is older than the latest version : {latestVer}");
                return results;
            }
        }
        context.FinalDirectory = 
            context.LevelDirectory.CreateSubdirectory($"v{context.RootComponent.AuthoredMetadata.ContentVer:D3}");
        results.SubmitResults(true, "Version resolution completed");
        return results;
    }

    static void ResolveImageCompiler(ExportContext context)
    {
        if (context.RootComponent.IsThumbnailValid())
            context.ImageResolver = new FileToImage(context.RootComponent.AuthoredMetadata.ThumbnailPath);
        else 
            context.ImageResolver = new TextureToImage(context.Settings.DefaultThumbnail);
    }
    #endregion
    
    #region Compiler functions
    static void CompileExport(ExportContext context)
    {
        ExportDisplay.StartStage(ExportStage.Compiling);
        context.Metadata = context.RootComponent.GetLevelMetadata();
        
        ExportDisplay.UpdateTask(CompilationStage.CompilingThumbnail);
        context.ThumbnailImage = context.ImageResolver.GetImage();
        
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
        string thumbnailPath = Path.Combine
            (context.FinalDirectory.FullName, $"thumbnail.png");
        File.WriteAllBytes(thumbnailPath, context.ThumbnailImage);
    }
    #endregion

    #region Other functions

    static void DebugDisplayTaskResults(TaskResults results)
    {
        Debug.Log($"Task results: {results.Success}");
        Debug.Log(results.Caption);
        if (results.Warnings.Count > 0)
            Debug.LogWarning($"Warnings: " +
                             $"{string.Join("\n", results.Warnings)}");
        if (results.Errors.Count > 0)
            Debug.LogError($"Errors: " +
                           $"{string.Join("\n", results.Errors)}");
    }

    static void SignOffExport(string exportPath)
    {
       AssetDatabase.Refresh(); 
       string assetPath = LocalPaths.GetAssetPath(exportPath);
       Object exportedAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

       if (exportedAsset)
       {
           Selection.activeObject = exportedAsset;
           EditorGUIUtility.PingObject(exportedAsset);
           return;
       }
       else
       {
           Process.Start("explorer.exe", exportPath);
       }
       
    }
    #endregion
}
