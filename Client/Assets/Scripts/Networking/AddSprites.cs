using System;

[Serializable]
public class AddSpritesRequest
{
    public string author;
    public string content;
    public string description;
    public uint dimensionX;
    public uint dimensionY;
    public string name;
    public string[] tags;
}