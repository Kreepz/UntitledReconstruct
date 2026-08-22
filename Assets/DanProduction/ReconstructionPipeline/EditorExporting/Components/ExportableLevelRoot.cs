using System;
using Unity.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEditor;

public class ExportableLevelRoot : MonoBehaviour
{
    [SerializeField, CreateProperty] LevelAuthorMetadata authoredMetadata;
    public LevelAuthorMetadata AuthoredMetadata => authoredMetadata;

    public void SetLevelThumbnail(string path)
    {
        authoredMetadata.ThumbnailPath = path;
    }
    
    public TaskResults ValidateHierarchy()
    {
        TaskResults results = new();
        results.SubmitResults(true, "Yet to add any rules but working so far!!");
        return results;
    }

    public LevelMetadata GetLevelMetadata()
    {
        return new LevelMetadata(authoredMetadata);
    }
}
