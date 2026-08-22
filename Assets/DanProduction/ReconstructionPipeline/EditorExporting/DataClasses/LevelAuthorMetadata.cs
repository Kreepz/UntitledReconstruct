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
    [SerializeField, CreateProperty, HideInInspector] public string ThumbnailPath;
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

    public TaskResults ValidateData()
    {
        TaskResults results = new();
        
        //warnings
        if (string.IsNullOrEmpty(LevelDesc))
            results.Warnings.Add("Level description field is empty");
        
        //error cases
        if (string.IsNullOrEmpty(LevelName)) 
            results.Errors.Add("Level name field is empty");
        if (string.IsNullOrEmpty(LevelId)) 
            results.Errors.Add("Level ID is empty");
        if (!Thumbnail)
            results.Errors.Add("Thumbnail field is empty");

        //results resolution
        if (results.Errors.Count > 0)
            results.SubmitResults(false, "Data validation failed");
        else
            results.SubmitResults(true, "Data validation passed");
        
        return results;
    }
}
