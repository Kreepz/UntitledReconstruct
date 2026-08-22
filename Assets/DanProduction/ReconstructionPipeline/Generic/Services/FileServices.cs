using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileServices
{
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
}
