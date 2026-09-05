using System;
using System.IO;
using UnityEngine;

public static class LocalPaths
{
    //Application runtime paths
    public static string LibraryName = "LibraryCatalogue";
    public static string ContentName = "ContentCatalogue";
    
    public static string CataloguePath => 
        Path.Combine(Application.persistentDataPath, "Catalogue");
    public static string ContentPath =>
        Path.Combine(Application.persistentDataPath, "Content");
    
    //Editor paths
    public static string ExportPath =>
        Path.Combine(Application.dataPath, "LevelExports");
    
    //Export variables
    public static DirectoryInfo EditorExport => 
        new(Path.Combine(Application.dataPath, "LevelExports"));
    public static DirectoryInfo ShipExport =>
        new(Path.Combine(Application.streamingAssetsPath, "ShippedLevels"));
    
    public static string GetAssetPath(string path)
    {
        int assetIndex = path.IndexOf("Assets", StringComparison.Ordinal);
        if (assetIndex < 0) return null;
        
        return path.Substring(assetIndex).
            Replace("\\", "/");
    }

    public static bool IsInStreamingAssets(string path)
    {
        int results = path.IndexOf("StreamingAssets", StringComparison.Ordinal);
        return results > 0;
    }
}
