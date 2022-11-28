using System;

[Serializable]
public class GetAllSpritesRequest
{
    public uint limit = 0;
    public bool asc = false;
    public string tags = string.Empty;
    public string author = string.Empty;
    public string name = string.Empty;
    public uint dimensionXMin = 0;
    public uint dimensionXMax = 0;
    public uint dimensionYMin = 0;
    public uint dimensionYMax = 0;
    public uint ratingMin = 0;
    public uint ratingMax = 0;
    public string lastItem = string.Empty;
}