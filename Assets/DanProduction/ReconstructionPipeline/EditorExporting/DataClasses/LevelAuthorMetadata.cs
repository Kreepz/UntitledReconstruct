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
    [SerializeField, HideInInspector] int contentVer;
    public int ContentVer
    {
        get => contentVer;
        set => contentVer = Mathf.Max(1, value);
    }

    [SerializeField, CreateProperty, HideInInspector] public float RequiredAppVer;
    [SerializeField, CreateProperty, HideInInspector] public string LevelId;

    public void GenerateNewId(bool forceRewrite = false)
    {
        if(!string.IsNullOrEmpty(LevelId) && !forceRewrite)return;
        LevelId = Guid.NewGuid().ToString("N");
        Debug.Log($"Generated new level ID: {LevelId}");
    }

    public bool ValidateData()
    {
        bool results = true;
        
        //warnings
        if (string.IsNullOrEmpty(LevelDesc))
        {
            Debug.LogWarning("Level description field is empty");
        }
        //error cases
        if (string.IsNullOrEmpty(LevelName))
        {
            Debug.LogError("Level name field is empty");
            results = false;
        }
        if (string.IsNullOrEmpty(LevelId))
        {
            Debug.LogError("Level ID description field is empty");
            results = false;
        }
        if (!Thumbnail)
        {
            Debug.LogError("Thumbnail field is empty");
            results = false;
        }
        
        return results;
    }
}
