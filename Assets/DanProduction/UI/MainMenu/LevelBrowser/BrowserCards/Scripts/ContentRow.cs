using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ContentRow : VisualElement
{
    List<ContentCard> _contentCards = new();

    public void InitialiseElement()
    {
        _contentCards = this.Query<ContentCard>().ToList();
        foreach (ContentCard card in _contentCards)
        {
            card.InitCard();
        }
    }
    
    public void BindCards(List<ContentCardData> rowList)
    {
        foreach (ContentCard card in _contentCards) 
            card.ClearCard();
        
        for (int i = 0; i< rowList.Count && i < _contentCards.Count; i++)
            _contentCards[i].BindData(rowList[i]);
    }
}
