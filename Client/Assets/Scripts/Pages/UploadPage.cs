using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
using SFB;

public class UploadPage
{
    private static UploadPage m_Instance = null;
    public static UploadPage Instance
    {
        get
        {
            if( m_Instance == null )
            {
                m_Instance = new UploadPage();
            }
            return m_Instance;
        }
    }
    private bool m_IsInit = false;

    // Variables
    private UploadPageReferencer m_Referencer;
    private const float m_ErrorMessageStay = 2f;

    // Functions

    private UploadPage()
    {
        // Get Referencer
        m_Referencer = Referencer.UploadPage;

        // Button action
        m_Referencer.BackButton.onClick.AddListener( () => CanvasManager.Instance.SwitchCanvas( CanvasManager.Canvases.Welcome ) );
        m_Referencer.ImportButton.onClick.AddListener( ImportButtonLogic );
        m_Referencer.ShareButton.onClick.AddListener( ShareButtonLogic );
    }

    public void Init()
    {
        if( m_IsInit )
        {
            return;
        }
        m_IsInit = true;
    }

    private void ImportButtonLogic()
    {
        // Open file with filter
        var extensions = new [] {
            new ExtensionFilter("Image Files", "png" ),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if ( paths.Length != 0 && paths[0].Length != 0 )
        {
            // Extract the texture
            string path = paths[0];
            var fileContent = File.ReadAllBytes( path );
            Texture2D tempTexture = new Texture2D( 2, 2 );
            tempTexture.LoadImage(fileContent);

            // Convert the texture
            Texture2D finalTexture = new Texture2D( tempTexture.width, tempTexture.height, TextureFormat.RGBA32, false );
            finalTexture.SetPixels( tempTexture.GetPixels() );
            finalTexture.filterMode = FilterMode.Point;
            finalTexture.Apply();

            // Create the sprite
            Sprite sprite = Sprite.Create( finalTexture, new Rect( 0, 0, finalTexture.width, finalTexture.height ), Vector2.one * 0.5f );
            m_Referencer.SpriteContent.sprite = sprite;
        }
    }

    private async void ShareButtonLogic()
    {
        m_Referencer.ShareButton.gameObject.SetActive(false);
        GameManager.Instance.StopCoroutine( ClearErrorMessage() );

        m_Referencer.ErrorMessage.text = "Sharing...";

        // Errors
        if( m_Referencer.SpriteContent.sprite == null )
        {
            ExitShareLogic( "Sprites not provided." );
            return;
        }
        else if( string.IsNullOrWhiteSpace( m_Referencer.Author.text ) )
        {
            ExitShareLogic( "Author not provided." );
            return;
        }
        else if( string.IsNullOrWhiteSpace( m_Referencer.Description.text ) )
        {
            ExitShareLogic( "Name not provided." );
            return;
        }
        else if( string.IsNullOrWhiteSpace( m_Referencer.Name.text ) )
        {
            ExitShareLogic( "Description not provided." );
            return;
        }

        // Extract tags
        string[] tagArray = new string[0];
        if( !string.IsNullOrWhiteSpace( m_Referencer.Tags.text ) )
        {
            tagArray = m_Referencer.Tags.text.Split( ',' );
        }
        
        // Send logic
        try
        {
            await SpritesController.AddSprites( new AddSpriteRequest(){
                author = m_Referencer.Author.text,
                content = ConvertManager.GetStringFromTexture( m_Referencer.SpriteContent.sprite.texture ),
                description = m_Referencer.Description.text,
                dimensionX = (uint) m_Referencer.SpriteContent.sprite.texture.width,
                dimensionY = (uint) m_Referencer.SpriteContent.sprite.texture.height,
                name = m_Referencer.Name.text,
                tags = tagArray,
            } );
        }
        catch( System.Exception e )
        {
            ExitShareLogic( e.Message );
            return;
        }

        ExitShareLogic( "Shared!" );
    }

    private void ExitShareLogic( string errorMessage )
    {
        m_Referencer.ErrorMessage.text = errorMessage;
        m_Referencer.ShareButton.gameObject.SetActive(true);

        GameManager.Instance.StartCoroutine( ClearErrorMessage() );
    }

    private IEnumerator ClearErrorMessage()
    {
        yield return new WaitForSeconds( m_ErrorMessageStay );
        m_Referencer.ErrorMessage.text = string.Empty;
    }
}
