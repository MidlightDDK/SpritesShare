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
    private bool m_IsInit = false;

    // Variables
    private WelcomePageReferencer m_Referencer;
    private const uint m_SpriteCount = 30;
    private const float m_AppearInterval = 0.5f;

    // Functions

    private WelcomePage()
    {
        // Get Referencer
        m_Referencer = Referencer.WelcomePage;

        // Button action
        m_Referencer.BrowseButton.onClick.AddListener( () => CanvasManager.Instance.SwitchCanvas( CanvasManager.Canvases.Browse ) );
        m_Referencer.UploadButton.onClick.AddListener( () => CanvasManager.Instance.SwitchCanvas( CanvasManager.Canvases.Upload ) );
        m_Referencer.ToggleMusicButton.onClick.AddListener( ToggleMusicLogic );
        m_Referencer.ToggleMusicText.text = "Stop Music";
        m_Referencer.ExitButton.onClick.AddListener( Application.Quit );
    }

    public void Init()
    {
        if( m_IsInit )
        {
            return;
        }
        m_IsInit = true;

        LoadSprites();
    }

    private void ToggleMusicLogic()
    {
        if( m_Referencer.MusicPlayer.isPlaying )
        {
            m_Referencer.MusicPlayer.Stop();
            m_Referencer.ToggleMusicText.text = "Play Music";
        }
        else
        {
            m_Referencer.MusicPlayer.Play();
            m_Referencer.ToggleMusicText.text = "Stop Music";
        }
    }

    private async void LoadSprites()
    {
        SpritesObjectController.GetAllSpritesResponse = await SpritesController.GetAllSprites();
        GameManager.Instance.StartCoroutine( SpawnSprite() );
    }

    IEnumerator SpawnSprite()
    {
        GameObject spritesContainer = new GameObject( "WelcomeSpritesContainer" );
        spritesContainer.transform.position = Vector3.zero;

        for( uint i = 0; i < m_SpriteCount; i++ )
        {
            GameObject spritesPrefab = GameObject.Instantiate( Resources.Load<GameObject>( ResourcesManager.GetPrefabPath( ResourcesManager.PrefabPath.Sprites ) ), spritesContainer.transform );
            yield return new WaitForSeconds( m_AppearInterval );
        }
    }
}
