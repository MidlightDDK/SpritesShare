using System;
using System.Collections.Generic;


[Serializable]
public class SpriteData
{
    public string author;
    public string content;
    public string dateCreated;
    public string description;
    public uint dimensionX;
    public uint dimensionY;
    public string id;
    public string name;
    public uint rating;
    public Dictionary<string, uint> ratings;
    public string[] tags;
}

