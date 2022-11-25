using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomePage
{
    private static WelcomePage m_Instance = null;
    public static WelcomePage Instance
    {
        get
        {
            if( m_Instance == null )
            {
                m_Instance = new WelcomePage();
            }
            return m_Instance;
        }
    }

    // Variables
    public Sprites[] GetAllSpritesResponse = null;

    // Functions

    private WelcomePage()
    {
        
    }

    public async void LoadSprites()
    {
        GetAllSpritesResponse = await SpritesController.GetAllSprites();

        GameObject spritesPrefab = GameObject.Instantiate( Resources.Load<GameObject>( ResourcesManager.GetPrefabPath( ResourcesManager.PrefabPath.Sprites ) ) );
        SpriteRenderer spriteRenderer = spritesPrefab.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ConvertManager.GetSpriteFromString( GetAllSpritesResponse[0].content, GetAllSpritesResponse[0].dimensionX, GetAllSpritesResponse[0].dimensionY );
    }
}
