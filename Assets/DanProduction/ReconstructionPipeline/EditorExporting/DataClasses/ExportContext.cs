using System.IO;
using UnityEngine;

public class ExportContext
{
    #region Input variables
    
    public ExportableLevelRoot RootComponent {get;}
    public ExportSettings Settings {get;}
    public DirectoryInfo DepositDirectory { get; }
    
    #endregion
    
    #region Generated variables
    
    //Directories
    public DirectoryInfo LevelDirectory { get; set; }
    public DirectoryInfo FinalDirectory { get; set; }
    
    //resolvers
    public IImageResolver ImageResolver { get; set; }
    
    //Deploy-ready formats
    public LevelMetadata Metadata { get; set; }
    public byte[] ThumbnailImage { get; set; }
    #endregion
    
    
    public ExportContext(ExportableLevelRoot rootComponent, ExportSettings settings, DirectoryInfo depositDirectory)
    {
        RootComponent = rootComponent;
        Settings = settings;
        DepositDirectory = depositDirectory;
    }
}
