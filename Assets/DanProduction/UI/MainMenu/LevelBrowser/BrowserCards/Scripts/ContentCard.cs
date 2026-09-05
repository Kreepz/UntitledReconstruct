using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ContentCard : VisualElement
{
    ContentCardData _assignedData;
    
    //ui elements
    Image _thumbnail;
    Label _title;
    Label _author;
    
    public ContentCard()
    {
        RegisterCallback<ClickEvent>(_ => _assignedData?.TriggerInteraction());
    }
    
    #region Initialisation
    public void InitCard()
    {
        _thumbnail = this.Q<Image>("thumbnail");
        _title = this.Q<Label>("level-name");
        _author = this.Q<Label>("credits-text");
        
        ClearCard();
    }
    
    public void BindData(ContentCardData source)
    {
        _assignedData = source;
        _thumbnail.image = source.Thumbnail;
        _title.text = source.Metadata.LevelName;
        _author.text = $"Created by: {source.Metadata.Author}";
        
        SetVisible(true);
    }
    

    #endregion
    
    #region State management
    void SetVisible(bool toggle)
    {
        visible = toggle;
        SetEnabled(toggle);
    }
    
    public void ClearCard()
    {
        _assignedData = null;
        _thumbnail.image = null;
        _title.text = "";
        _author.text = "";
        SetVisible(false);
    }
    

    #endregion

    #region Functionality

    void PromptContentPreview()
    {
        
    }
    
    #endregion
    
}
