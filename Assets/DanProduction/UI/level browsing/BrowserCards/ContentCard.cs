using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ContentCard : VisualElement
{
    LevelMetadata _assignedData;
    
    //ui elements
    Image _thumbnail;
    Label _title;
    Label _author;

    public ContentCard()
    {
        _thumbnail = this.Q<Image>("thumbnail");
        _title = this.Q<Label>("level-name");
        _author = this.Q<Label>("credits-text");
        
        //ClearCard();
    }

    public void InitCard()
    {
        _thumbnail = this.Q<Image>("thumbnail");
        _title = this.Q<Label>("level-name");
        _author = this.Q<Label>("credits-text");
    }
    
    public void BindMetadata(LevelMetadata source)
    {
        _assignedData = source;
        _thumbnail.image = source.GetThumbnail();
        _title.text = source.LevelName;
        _author.text = $"Created by: {source.Author}";
        
        SetVisible(true);
    }

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
}
