using System;
using Unity.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEditor;

public class ExportableLevelRoot : MonoBehaviour
{
    [SerializeField, CreateProperty] LevelAuthorMetadata authoredMetadata;
    public LevelAuthorMetadata AuthoredMetadata => authoredMetadata;

    public void ExportLevel()
    {
        LevelMetadata metadata = new LevelMetadata(authoredMetadata);
        LocalExporter.ExportLevel(metadata, authoredMetadata.Thumbnail);
    }
}
