using System;

public static class FileServices
{
    public static int GetVersionNumber(string folderName)
    {
        //expected format : v001
        if (folderName.StartsWith("v", StringComparison.Ordinal) &&
            int.TryParse(folderName.Substring(1), out int version))
        {
            return version;
        }
        return -1;
    }
}
