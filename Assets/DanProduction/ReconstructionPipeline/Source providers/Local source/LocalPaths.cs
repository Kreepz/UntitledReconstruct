using System.IO;
using UnityEngine;

public static class LocalPaths
{
    //Application runtime paths
    public static string LibraryName = "LibraryCatalogue";
    public static string ContentName = "ContentCatalogue";
    
    public static string LibraryPath => 
        Path.Combine(Application.persistentDataPath, "Library");
    public static string ContentPath =>
        Path.Combine(Application.persistentDataPath, "Content");
    public static string ShippedLibraryPath =>
        Path.Combine(Application.streamingAssetsPath, "Library");
    public static string ShippedContentPath =>
        Path.Combine(Application.streamingAssetsPath, "Content");
    
    
    //Editor paths
    public static string ExportPath = "Assets/LevelExports";

}
