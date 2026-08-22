using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ContentRow : VisualElement
{
    List<ContentCard> _contentCards = new();

    public void BindCards(List<LevelMetadata> rowList)
    {
        if (_contentCards.Count == 0)
            _contentCards = this.Query<ContentCard>().ToList();
        foreach (ContentCard card in _contentCards)
        {
            card.InitCard();
            Debug.Log($"Clearing card ; {card}");
            card.ClearCard();
        }
        
        for (int i = 0; i< rowList.Count && i < _contentCards.Count; i++)
            _contentCards[i].BindMetadata(rowList[i]);
    }
}
