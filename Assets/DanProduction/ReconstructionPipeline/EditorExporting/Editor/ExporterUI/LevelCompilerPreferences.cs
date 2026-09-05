using UnityEditor;

public static class LevelCompilerPreferences
{
    const string AutoVersionKey = "LevelCompiler.AutoVersion";

    public static bool AutoVersion
    {
        get => EditorPrefs.GetBool(AutoVersionKey, true);
        set => EditorPrefs.SetBool(AutoVersionKey, value);
    }
}
