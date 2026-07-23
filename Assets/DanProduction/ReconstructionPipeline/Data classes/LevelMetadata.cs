using UnityEngine;

public class LevelMetadata
{
    //Profile data
    public readonly string LevelName;
    public readonly string LevelDescription;
    public readonly Authors Author;
    
    //System data
    public readonly string ContentID;
    public readonly int ContentVersion;
    public readonly float RequiredAppVersion;
    
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
}
