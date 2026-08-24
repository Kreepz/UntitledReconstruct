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
}

public class PageData
{
    public List<RowData> Rows;
}