using UnityEngine;

public class LevelMetadata
{
    //Profile data
    public readonly string LevelName;
    public readonly string LevelDescription;
    public readonly Authors Author;
    
    //System data
    public readonly string ContentID;
    public int ContentVersion {get; private set;}
    public float RequiredAppVersion{get; private set;}
    
    //misc
    public readonly bool Official;
    
    
    public LevelMetadata(LevelAuthorMetadata authoredMetadata)
    {
        ContentID = authoredMetadata.LevelId;
        LevelName = authoredMetadata.LevelName;
        LevelDescription = authoredMetadata.LevelDesc;
        Author =  authoredMetadata.Author;
        ContentVersion = authoredMetadata.ContentVer;
        RequiredAppVersion = authoredMetadata.RequiredAppVer;
    }
    
    public void SetContentVersion(int contentVersion)
    {
        ContentVersion = contentVersion;
    }
}
