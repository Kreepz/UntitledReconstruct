using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentPaginator
{
    [Header("Visual structure")]
    [SerializeField] int cardPerRow = 5;
    [SerializeField] int rowPerPage = 6;
    
    public List<PageData> _pages = new List<PageData>();
    public int currentPage;
    public PageData CurrentPage => _pages[currentPage];
    
    public void BuildPages(List<LevelMetadata> source)
    {
        _pages.Clear();
        currentPage = 0;

        int sourceIndex = 0;

        while (sourceIndex < source.Count)
        {
            //build page
            PageData page = new()
            {
                Rows = new()
            };

            for (int rowIndex = 0;
                 rowIndex < rowPerPage && sourceIndex < source.Count;
                 rowIndex++)
            {
                RowData row = new()
                {
                    Contents = new()
                };

                //build row
                for (int cardIndex = 0;
                     cardIndex < cardPerRow && sourceIndex < source.Count;
                     cardIndex++)
                {
                    row.Contents.Add(source[sourceIndex]);
                    sourceIndex++;
                }
                page.Rows.Add(row);
            }
            _pages.Add(page);
        }
    }
}
