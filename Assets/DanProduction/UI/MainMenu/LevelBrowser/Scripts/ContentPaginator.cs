using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentPaginator
{
    [Header("Visual structure")]
    [SerializeField] int cardPerRow = 5;
    [SerializeField] int rowPerPage = 6;
    
    
    List<PageData> _pages = new();
    int _currentPage;
    
    //Getters
    public PageData CurrentPage => 
        _currentPage >= 0 && _currentPage < _pages.Count 
            ? _pages[_currentPage] : null;

    public int CurrentPageNumber => _currentPage + 1;
    public int PageCount => _pages.Count;
    public bool HasNextPage => _currentPage < _pages.Count - 1;
    public bool HasPreviousPage => _currentPage > 0;
    
    public void BuildPages(PaginationContext context)
    {
        _pages.Clear();
        if(context.ResetPage) 
            _currentPage = 0;
        
        int sourceIndex = 0;
        int sourceCount = context.Source.Count;
        
        while (sourceIndex < sourceCount)
        {
            //build page
            PageData page = new()
            {
                Rows = new()
            };

            for (int rowIndex = 0;
                 rowIndex < rowPerPage && sourceIndex < sourceCount;
                 rowIndex++)
            {
                RowData row = new()
                {
                    Contents = new()
                };

                //build row
                for (int cardIndex = 0;
                     cardIndex < cardPerRow && sourceIndex < sourceCount;
                     cardIndex++)
                {
                    ContentCardData cardData = new()
                    {
                        Metadata = context.Source[sourceIndex],
                        FetchThumbnail = context.ThumbnailResolver,
                        OnCardInteract = context.OnCardInteract
                    };
                    row.Contents.Add(cardData);
                    sourceIndex++;
                }
                page.Rows.Add(row);
            }
            _pages.Add(page);
        }

        //Prevent out of index exemptions
        if (!context.ResetPage && _currentPage >= _pages.Count)
        {
            _currentPage = _pages.Count - 1;
        }
    }

    public void NextPage()
    {
        if(_currentPage < _pages.Count - 1)
            _currentPage++;
    }

    public void PreviousPage()
    {
        if (_currentPage > 0) 
            _currentPage--;
    }
}
