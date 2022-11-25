using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ConvertManager
{
    public const TextureFormat AcceptedTextureFormat = TextureFormat.RGBA32;

    public static string GetStringFromTexture( Texture2D texture2D )
    {
        if( texture2D.format != AcceptedTextureFormat )
        {
            throw new System.Exception( "Unsupported texture format." );
        }

        byte[] byteArray = texture2D.GetRawTextureData();
        string base64String = Convert.ToBase64String(byteArray);
        return base64String;
    }

    public static Texture2D GetTextureFromString( string encodedString, uint dimensionX, uint dimensionY )
    {
        byte[] newByteArray = Convert.FromBase64String(encodedString);
        Texture2D newTexture = new Texture2D( (int) dimensionX, (int) dimensionY, AcceptedTextureFormat, false );
        newTexture.LoadRawTextureData( newByteArray );
        newTexture.Apply();
        return newTexture;
    }
}
