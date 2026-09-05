using System.IO;
using UnityEngine;

public interface IImageResolver
{
    public byte[] GetImage();
}

public class FileToImage : IImageResolver
{
    readonly string _path;
    public FileToImage(string filePath)
    {
        _path = filePath;
    }
    public byte[] GetImage()
    {
        return File.ReadAllBytes(_path);
    }
}

public class TextureToImage : IImageResolver
{
    readonly Texture2D _texture;

    public TextureToImage(Texture2D texture)
    {
        _texture = texture;
    }

    public byte[] GetImage()
    {
        return _texture.EncodeToPNG();
    }
}
