using UnityEngine;
using System;

public static class ConvertManager
{
    public const TextureFormat AcceptedTextureFormat = TextureFormat.RGBA32;
    public const uint SpritesPixelPerUnit = 16;

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
        newTexture.filterMode = FilterMode.Point;
        newTexture.LoadRawTextureData( newByteArray );
        newTexture.Apply();
        return newTexture;
    }

    public static Sprite GetSpriteFromString( string encodedString, uint dimensionX, uint dimensionY )
    {
        Texture2D newTexture = GetTextureFromString( encodedString, dimensionX, dimensionY );
        Sprite newSprite = Sprite.Create( newTexture, new Rect( 0, 0, newTexture.width, newTexture.height ), new Vector2( 0.5f, 0.5f ), SpritesPixelPerUnit );
        return newSprite;
    }
}
