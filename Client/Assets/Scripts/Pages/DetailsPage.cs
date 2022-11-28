using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
using SFB;

public class DetailsPage
{
    private static DetailsPage m_Instance = null;
    public static DetailsPage Instance
    {
        get
        {
            if( m_Instance == null )
            {
                m_Instance = new DetailsPage();
            }
            return m_Instance;
        }
    }
    private bool m_IsInit = false;

    // Variables
    private DetailsPageReferencer m_Referencer;
    private uint m_Rating = m_DefaultRating;
    private const uint m_DefaultRating = 3;
    private const float m_ErrorMessageStay = 2f;
    private readonly Color m_ColorStarActivated = Color.white;
    private readonly Color m_ColorStarDisabled = Color.grey;
    private const uint m_StarCount = 5;
    private const string m_NotAssigned = "n/a";
    private SpriteData m_SpritesData = null;

    // Functions

    private DetailsPage()
    {
        // Get Referencer
        m_Referencer = Referencer.DetailsPage;

        // Button action
        m_Referencer.BackButton.onClick.AddListener( () => CanvasManager.Instance.SwitchCanvas( CanvasManager.Canvases.Browse ) );
        m_Referencer.RateButton.onClick.AddListener( RateButtonLogic );
        m_Referencer.DownloadButton.onClick.AddListener( DownloadButtonLogic );
        for( uint i = 0; i < m_StarCount; i++ )
        {
            uint currentRating = i + 1;
            m_Referencer.StarButtons[ i ].onClick.AddListener( () => SetRating( currentRating ) );
        }
    }

    public void Init()
    {
        if( m_IsInit )
        {
            return;
        }
        m_IsInit = true;
    }

    public void LoadSpriteData( SpriteData spritesData )
    {
        m_SpritesData = spritesData;

        // Common data
        m_Referencer.Id.text = m_SpritesData.id;
        m_Referencer.Author.text = m_SpritesData.author;
        m_Referencer.Name.text = m_SpritesData.name;
        m_Referencer.DateCreated.text = m_SpritesData.dateCreated.Replace( "T", " " ).Replace( "Z", string.Empty );
        m_Referencer.Name.text = m_SpritesData.name;
        m_Referencer.Description.text = m_SpritesData.description;

        // Tags
        if( m_SpritesData.tags != null && m_SpritesData.tags.Length > 0 )
        {
            m_Referencer.Tags.text = string.Join( ',', m_SpritesData.tags );
        }
        else
        {
            m_Referencer.Tags.text = m_NotAssigned;
        }

        // Global Rating
        if( m_SpritesData.rating > 0 )
        {
            m_Referencer.GlobalRating.text = m_SpritesData.rating + "/5";
        }
        else
        {
            m_Referencer.GlobalRating.text = m_NotAssigned;
        }

        // Sprites
        m_Referencer.SpriteContent.sprite = ConvertManager.GetSpriteFromString( m_SpritesData.content, m_SpritesData.dimensionX, m_SpritesData.dimensionY );

        // Default rating
        SetRating( m_DefaultRating );
    }

    private void SetRating( uint newRating )
    {
        // Initial checks
        if( newRating < 1 || newRating > m_StarCount )
        {
            throw new System.Exception( "Unsupported rating..." );
        }
        else if( m_Referencer.StarButtons.Length != m_StarCount )
        {
            throw new System.Exception( "Star Button count should be " + m_StarCount + "!" );
        }
        else if( m_Referencer.StarImages.Length != m_StarCount )
        {
            throw new System.Exception( "Star Image count should be " + m_StarCount + "!" );
        }

        m_Rating = newRating;
        for( uint i = 0; i < m_StarCount; i++ )
        {
            if( i + 1 <= m_Rating )
            {
                m_Referencer.StarImages[ i ].color = m_ColorStarActivated;
            }
            else
            {
                m_Referencer.StarImages[ i ].color = m_ColorStarDisabled;
            }
        }
    }

    private async void RateButtonLogic()
    {
        m_Referencer.RateButton.gameObject.SetActive( false );
        m_Referencer.DownloadButton.gameObject.SetActive( false );
        GameManager.Instance.StopCoroutine( ClearErrorMessage() );

        m_Referencer.ErrorMessage.text = "Rating...";

        try
        {
            await SpritesController.RateSprites( m_Referencer.Id.text, new RateSpriteRequest()
            {
                rating = m_Rating
            } );
        }
        catch( System.Exception e )
        {
            ExitErrorMessageLogic( e.Message );
            return;
        }

        // Update rating
        /*
        string ipv4 = IPManager.GetIP( ADDRESSFAM.IPv4 ).Replace( ".", " " );
        m_SpritesData.ratings[ ipv4 ] = m_Rating;
        uint totalRating = 0;
        uint ratingCount = 0;
        foreach( var currentRating in m_SpritesData.ratings )
        {
            ratingCount++;
            Debug.Log( "Rating " + currentRating.Key );
            totalRating += currentRating.Value;
        }
        totalRating /= ratingCount;
        m_Referencer.GlobalRating.text = totalRating + "/5";
        */

        ExitErrorMessageLogic( "Rated!" );
    }

    private void DownloadButtonLogic()
    {
        m_Referencer.RateButton.gameObject.SetActive( false );
        m_Referencer.DownloadButton.gameObject.SetActive( false );
        GameManager.Instance.StopCoroutine( ClearErrorMessage() );

        m_Referencer.ErrorMessage.text = "Downloading...";

        Texture2D texture = m_Referencer.SpriteContent.sprite.texture;

        // Save file with filter
        var extensionList = new [] {
            new ExtensionFilter("PNG", "png"),
        };
        var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", m_SpritesData.name, extensionList);

        if (path.Length != 0)
        {
            var pngData = texture.EncodeToPNG();
            if (pngData != null)
                File.WriteAllBytes(path, pngData);
            ExitErrorMessageLogic( "Downloaded!" );
        }
        else
        {
            ExitErrorMessageLogic( "Download canceled..." );
        }
    }

    private void ExitErrorMessageLogic( string errorMessage )
    {
        m_Referencer.ErrorMessage.text = errorMessage;
        m_Referencer.RateButton.gameObject.SetActive( true );
        m_Referencer.DownloadButton.gameObject.SetActive( true );

        GameManager.Instance.StartCoroutine( ClearErrorMessage() );
    }

    private IEnumerator ClearErrorMessage()
    {
        yield return new WaitForSeconds( m_ErrorMessageStay );
        m_Referencer.ErrorMessage.text = string.Empty;
    }
}
