using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance = null;

    private void Awake() 
    {
        if( Instance == null )
        {
            Instance = this;
        }
        else if( Instance != this )
        {
            Destroy( gameObject );
        }
    }

    // Variables

    public SpriteRenderer MyRenderer;
    public Texture2D MyTexture;

    private void Start()
    {
        // PNG to string
        byte[] byteArray = MyTexture.GetRawTextureData();
        string base64String = Convert.ToBase64String(byteArray);

        // String to png
        byte[] newByteArray = Convert.FromBase64String(base64String);
        Debug.Log( MyTexture.format );
        Texture2D newTexture = new Texture2D( MyTexture.width, MyTexture.height, TextureFormat.RGBA32, false );
        newTexture.LoadRawTextureData(newByteArray);
        newTexture.Apply();

        // Apply it
        Sprite newSprite = Sprite.Create( newTexture, new Rect( 0, 0, newTexture.width, newTexture.height ), new Vector2( 0.5f, 0.5f ), 16 );
        MyRenderer.sprite = newSprite;
    }

    private void Update()
    {
        
    }

    private async void LoadSprites()
    {
        Sprites[] response = await SpritesController.GetAllSprites();
    }
}
