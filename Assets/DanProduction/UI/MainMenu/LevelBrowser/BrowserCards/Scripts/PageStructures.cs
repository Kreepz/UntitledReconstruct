using System;
using System.Collections.Generic;
using UnityEngine;

public class RowData
{
    public List<ContentCardData> Contents;
}

public class ContentCardData
{
    public LevelMetadata Metadata;
    public Func<LevelMetadata, Texture2D> FetchThumbnail;
    public Texture2D Thumbnail => FetchThumbnail(Metadata);
    public Action<LevelMetadata> OnCardInteract;

    public void TriggerInteraction()
    {
        OnCardInteract?.Invoke(Metadata);
    }
}

public class PageData
{
    public List<RowData> Rows;
}

public class PaginationContext
{
    public List<LevelMetadata> Source{get;set;}
    public Func<LevelMetadata, Texture2D> ThumbnailResolver{get;set;}
    public Action<LevelMetadata> OnCardInteract{get;set;}
    
}