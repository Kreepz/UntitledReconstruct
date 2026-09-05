using UnityEngine;

public static class ImageCompiler
{
    public static byte[] ToPng(Texture2D image)
    {
        return image.EncodeToPNG();
    }
}
