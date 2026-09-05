using UnityEngine;

public static class ExportUtility
{
    public static void ImportPreferences(this ExportSettings exportSettings)
    {
        exportSettings.AutomaticVersioning = LevelCompilerPreferences.AutoVersion;
    }
}
