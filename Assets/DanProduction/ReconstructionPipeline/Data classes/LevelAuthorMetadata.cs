using System;
using Unity.Collections;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LevelAuthorMetadata
{
    //Profile data
    [SerializeField, CreateProperty, HideInInspector] public string LevelName;
    [SerializeField, CreateProperty, HideInInspector] public string LevelDesc;
    [SerializeField, CreateProperty, HideInInspector] public Authors Author;
    [SerializeField, CreateProperty, HideInInspector] public Texture2D Thumbnail;

    //System data
    [SerializeField, CreateProperty, HideInInspector] public int ContentVer = 1;
    [SerializeField, CreateProperty, HideInInspector] public float RequiredAppVer;
    [SerializeField, CreateProperty, HideInInspector] public string LevelId;

    public void GenerateNewId(bool forceRewrite = false)
    {
        if(!string.IsNullOrEmpty(LevelId) && !forceRewrite)return;
        LevelId = Guid.NewGuid().ToString("N");
        Debug.Log($"Generated new level ID: {LevelId}");
    }
}
