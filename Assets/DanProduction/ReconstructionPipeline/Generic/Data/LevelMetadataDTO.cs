using UnityEngine;

public class LevelMetadataDTO
{
    public string LevelName;
    public string LevelDescription;
    public Authors Author;

    public string ContentID;
    public int ContentVersion;
    public float RequiredAppVersion;

    public bool Official;

    public LevelMetadataDTO(){}
    public LevelMetadataDTO(LevelAuthorMetadata authorableMetadata, bool isOfficial)
    {
        ContentID = authorableMetadata.LevelId;
        LevelName = authorableMetadata.LevelName;
        LevelDescription = authorableMetadata.LevelDesc;
        Author = authorableMetadata.Author;
        ContentVersion = authorableMetadata.ContentVer;
        RequiredAppVersion = authorableMetadata.RequiredAppVer;
        Official = isOfficial;
    }
}
