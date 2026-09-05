using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public static class FileServices
{
    public static string GetSafeFileName(string name)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();

        return new string(
            name
                .Where(c => !invalidChars.Contains(c))
                .ToArray()
        ).TrimEnd(' ', '.');
    }
    public static string GetVersionFolderFormat(int version)
    {
        return "";
    }
    public static int GetVersionNumber(string folderName)
    {
        //expected format : v001
        if (folderName.StartsWith("v", StringComparison.Ordinal) &&
            int.TryParse(folderName.Substring(1), out int version))
        {
            return version;
        }
        return -1;
    }
    
    public static Texture2D GetThumbnail(this LevelMetadata data)
    {
        var texture = new Texture2D(2, 2);
        string versionID = $"v{data.ContentVersion:D3}";
        
        FileInfo imageFile = new(Path.Combine(LocalPaths.ContentPath, data.ContentID, versionID, "thumbnail.png"));
        if (!File.Exists(imageFile.FullName))
        {
            Debug.LogError("Cannot find thumbnail");
            return null;
        }
        
        texture.LoadImage(File.ReadAllBytes(imageFile.FullName));
        return texture;
    }
    public static Texture2D GetPreviewThumbnail(this LevelAuthorMetadata data)
    {
        if(string.IsNullOrEmpty(data.ThumbnailPath))
            return null;
        
        var texture = new Texture2D(2, 2);
        FileInfo imageFile = new(data.ThumbnailPath);
        if (!File.Exists(imageFile.FullName))
        {
            Debug.LogError("Cannot find thumbnail");
            return null;
        }
        
        texture.LoadImage(File.ReadAllBytes(imageFile.FullName));
        return texture;
    }

    public static DirectoryInfo[] GetVersionFolders(DirectoryInfo levelRepo)
    {
        return levelRepo.GetDirectories()
            .Where(folder => GetVersionNumber(folder.Name) > 0)
            .OrderByDescending(folder => GetVersionNumber(folder.Name))
            .ToArray();
    }

    public static LevelMetadata GetMetaDataFile(string expectedPath)
    {
        if (!File.Exists(expectedPath))
        {
            Debug.LogError($"Metadata file not found at {expectedPath}");
            return null;
        }

        string json = File.ReadAllText(expectedPath);
        LevelMetadataDTO DTO = JsonConvert.DeserializeObject<LevelMetadataDTO>(json);
        if (DTO == null)
        {
            Debug.LogError("Failed to deserialise metadata");
            return null;
        }
        return new (DTO);
    }

    public static LevelCollection GetLevelCollectionFile(string expectedPath)
    {
        if (!File.Exists(expectedPath))
        {
            Debug.LogError($"Level file not found at {expectedPath}");
            return null;
        }

        string json = File.ReadAllText(expectedPath);

        //Apply settings
        JsonSerializerSettings settings = new();
        settings.Converters.Add(new Vector3Converter());
        settings.Converters.Add(new QuaternionConverter());
        settings.Converters.Add(new ReconstructableParameterConverter());
        
        LevelCollection level = JsonConvert.DeserializeObject<LevelCollection>(json, settings);
        if (level == null)
        {
            Debug.LogError("Failed to deserialise level collection");
            return null;
        }
        
        return level;
    }
}
