using Unity.Properties;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ExportSettings
{
    [CreateProperty]
    public bool AutomaticVersioning { get; set; }
}
