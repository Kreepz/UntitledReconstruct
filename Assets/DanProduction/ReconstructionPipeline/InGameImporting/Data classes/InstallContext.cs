using System.IO;
using UnityEngine;

public class InstallContext
{
    //Input 
    public DirectoryInfo LevelPackageDirectory {get; private set;}
    
    //Produced
    public DirectoryInfo[] Versions;
    public string ContentID;
    public LevelMetadata latestMetadata;
    public DirectoryInfo InstallationDirectory;
    public DirectoryInfo[] UninstalledVersions;

    //public string CataloguePath;
    //public DirectoryInfo[] 
    
    
    public InstallContext(DirectoryInfo levelPackageDirectory)
    {
        LevelPackageDirectory = levelPackageDirectory;
    }
}
