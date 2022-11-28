using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;

public class BrowsePage
{
    private static BrowsePage m_Instance = null;
    public static BrowsePage Instance
    {
        get
        {
            if( m_Instance == null )
            {
                m_Instance = new BrowsePage();
            }
            return m_Instance;
        }
    }
    private bool m_IsInit = false;

    // Variables
    private BrowsePageReferencer m_Referencer;
    private GetAllSpritesRequest m_GetSpritesRequest = new GetAllSpritesRequest()
    {
        limit = m_LimitMax
    };
    private bool m_IsDimensionX = true;
    private List<string> m_SpritesLastItems = new List<string>();
    private uint m_CurrentPage = 0;
    private const uint m_LimitMax = 24;

    // Functions

    private BrowsePage()
    {
        // Get Referencer
        m_Referencer = Referencer.BrowsePage;

        // Button action
        m_Referencer.BackButton.onClick.AddListener( () => CanvasManager.Instance.SwitchCanvas( CanvasManager.Canvases.Welcome ) );
        m_Referencer.SearchButton.onClick.AddListener( SearchButtonLogic );
        m_Referencer.PreviousButton.onClick.AddListener( PreviousButtonLogic );
        m_Referencer.DetailsButton.onClick.AddListener( DetailsButtonLogic );
        m_Referencer.NextButton.onClick.AddListener( NextButtonLogic );

        // Fields validate
        m_Referencer.AscendingField.onValueChanged.AddListener( ValidateAscending );
        m_Referencer.TagsField.onValueChanged.AddListener( ValidateTags );
        m_Referencer.AuthorField.onValueChanged.AddListener( ValidateAuthor );
        m_Referencer.NameField.onValueChanged.AddListener( ValidateName );
        m_Referencer.DimensionDropDown.onValueChanged.AddListener( ValidateDimensionDropDown );
        m_Referencer.DimensionMinField.onValueChanged.AddListener( ValidateDimensionMin );
        m_Referencer.DimensionMaxField.onValueChanged.AddListener( ValidateDimensionMax );
        m_Referencer.RatingMinField.onValueChanged.AddListener( ValidateRatingMin );
        m_Referencer.RatingMaxField.onValueChanged.AddListener( ValidateRatingMax );

        DisableNavigateButtons();
        m_Referencer.SearchButton.gameObject.SetActive( true );
    }

    private void ValidateAscending( bool newValue )
    {
        m_Referencer.AscendingField.onValueChanged.RemoveListener( ValidateAscending );

        m_GetSpritesRequest.asc = newValue;

        m_Referencer.AscendingField.onValueChanged.AddListener( ValidateAscending );
    }

    private void ValidateTags( string newValue )
    {
        m_Referencer.TagsField.onValueChanged.RemoveListener( ValidateTags );

        m_GetSpritesRequest.tags = newValue;

        m_Referencer.TagsField.onValueChanged.AddListener( ValidateTags );
    }

    private void ValidateAuthor( string newValue )
    {
        m_Referencer.AuthorField.onValueChanged.RemoveListener( ValidateAuthor );

        m_GetSpritesRequest.author = newValue;

        m_Referencer.AuthorField.onValueChanged.AddListener( ValidateAuthor );
    }

    private void ValidateName( string newValue )
    {
        m_Referencer.NameField.onValueChanged.RemoveListener( ValidateName );

        m_GetSpritesRequest.name = newValue;

        m_Referencer.NameField.onValueChanged.AddListener( ValidateName );
    }

    private void ValidateDimensionDropDown( int newValue )
    {
        m_Referencer.DimensionDropDown.onValueChanged.RemoveListener( ValidateDimensionDropDown );

        m_IsDimensionX = ( newValue == 0 ) ? true : false;
        if( m_IsDimensionX )
        {
            m_GetSpritesRequest.dimensionXMin = m_GetSpritesRequest.dimensionYMin;
            m_GetSpritesRequest.dimensionXMax = m_GetSpritesRequest.dimensionYMax;
            m_GetSpritesRequest.dimensionYMin = 0;
            m_GetSpritesRequest.dimensionYMax = 0;
        }
        else
        {
            m_GetSpritesRequest.dimensionYMin = m_GetSpritesRequest.dimensionXMin;
            m_GetSpritesRequest.dimensionYMax = m_GetSpritesRequest.dimensionXMax;
            m_GetSpritesRequest.dimensionXMin = 0;
            m_GetSpritesRequest.dimensionXMax = 0;
        }

        m_Referencer.DimensionDropDown.onValueChanged.AddListener( ValidateDimensionDropDown );
    }

    private void ValidateDimensionMin( string newValue )
    {
        m_Referencer.DimensionMinField.onValueChanged.RemoveListener( ValidateDimensionMin );

        uint dimension = 0;
        uint.TryParse( newValue, out dimension );
        if( m_IsDimensionX )
        {
            m_GetSpritesRequest.dimensionXMin = dimension;
        }
        else
        {
            m_GetSpritesRequest.dimensionYMin = dimension;
        }

        m_Referencer.DimensionMinField.onValueChanged.AddListener( ValidateDimensionMin );
    }

    private void ValidateDimensionMax( string newValue )
    {
        m_Referencer.DimensionMaxField.onValueChanged.RemoveListener( ValidateDimensionMax );

        uint dimension = 0;
        uint.TryParse( newValue, out dimension );
        if( m_IsDimensionX )
        {
            m_GetSpritesRequest.dimensionXMax = dimension;
        }
        else
        {
            m_GetSpritesRequest.dimensionYMax = dimension;
        }

        m_Referencer.DimensionMaxField.onValueChanged.AddListener( ValidateDimensionMax );
    }

    private void ValidateRatingMin( string newValue )
    {
        m_Referencer.RatingMinField.onValueChanged.RemoveListener( ValidateRatingMin );

        uint rating = 0;
        uint.TryParse( newValue, out rating );
        m_GetSpritesRequest.ratingMin = rating;

        m_Referencer.RatingMinField.onValueChanged.AddListener( ValidateRatingMin );
    }

    private void ValidateRatingMax( string newValue )
    {
        m_Referencer.RatingMaxField.onValueChanged.RemoveListener( ValidateRatingMax );

        uint rating = 0;
        uint.TryParse( newValue, out rating );
        m_GetSpritesRequest.ratingMax = rating;

        m_Referencer.RatingMaxField.onValueChanged.AddListener( ValidateRatingMax );
    }

    public void Init()
    {
        if( m_IsInit )
        {
            return;
        }
        m_IsInit = true;
    }


    private async void SearchButtonLogic()
    {
        DisableNavigateButtons();
        m_SpritesLastItems.Clear();
        m_CurrentPage = 0;
        m_GetSpritesRequest.lastItem = string.Empty;
        TransformFunctions.DestroyAllChildren( m_Referencer.GridContainer );

        m_Referencer.ErrorMessage.text = "Searching...";

        try
        {
            SpriteData[] spritesData = await SpritesController.GetAllSprites( m_GetSpritesRequest );
            if( spritesData != null && spritesData.Length > 0 )
            {
                LoadSpriteData( spritesData );
                string lastItemId = spritesData[ spritesData.Length - 1 ].id;
                m_SpritesLastItems.Add( lastItemId );

                // Is there a next page?
                uint currentLimit = m_GetSpritesRequest.limit;
                string currentLastItem = m_GetSpritesRequest.lastItem;
                try
                {
                    m_GetSpritesRequest.limit = 1;
                    m_GetSpritesRequest.lastItem = lastItemId;
                    SpriteData[] spritesData2 = await SpritesController.GetAllSprites( m_GetSpritesRequest );
                    if( spritesData2 != null && spritesData2.Length > 0 )
                    {
                        m_Referencer.NextButton.gameObject.SetActive( true );
                    }
                }
                catch( System.Exception e )
                {
                }
                m_GetSpritesRequest.limit = currentLimit;
                m_GetSpritesRequest.lastItem = currentLastItem;
                ExitSearchLogic( string.Empty );
            }
            else
            {
                ExitSearchLogic( "No result." );
            }
        }
        catch( System.Exception e )
        {
            ExitSearchLogic( e.Message );
            return;
        }
    }

    private void DisableNavigateButtons()
    {
        m_Referencer.SearchButton.gameObject.SetActive( false );
        m_Referencer.PreviousButton.gameObject.SetActive( false );
        m_Referencer.DetailsButton.gameObject.SetActive( false );
        m_Referencer.NextButton.gameObject.SetActive( false );
    }

    private void LoadSpriteData( SpriteData[] spritesData )
    {
        TransformFunctions.DestroyAllChildren( m_Referencer.GridContainer );
        for( int i = 0; i < spritesData.Length; i++ )
        {
            SpriteGridItemController spritesPrefab = GameObject.Instantiate( 
                Resources.Load<GameObject>( 
                    ResourcesManager.GetPrefabPath( ResourcesManager.PrefabPath.SpriteGridItem ) ), 
                    m_Referencer.GridContainer 
                ).GetComponent<SpriteGridItemController>();
            spritesPrefab.SetSpritesData( spritesData[i] );
        }
    }

    private async void PreviousButtonLogic()
    {
        DisableNavigateButtons();

        m_CurrentPage--;
        if( m_CurrentPage > 0 )
        {
            m_GetSpritesRequest.lastItem = m_SpritesLastItems[ (int) m_CurrentPage - 1 ];
        }
        else
        {
            m_GetSpritesRequest.lastItem = string.Empty;
        }

        try
        {
            SpriteData[] spritesData = await SpritesController.GetAllSprites( m_GetSpritesRequest );
            if( spritesData != null && spritesData.Length > 0 )
            {
                LoadSpriteData( spritesData );
            }
        }
        catch( System.Exception e )
        {
            ExitSearchLogic( e.Message );
        }
        
        m_Referencer.SearchButton.gameObject.SetActive( true );
        m_Referencer.NextButton.gameObject.SetActive( true );
        if( m_CurrentPage > 0 )
        {
            m_Referencer.PreviousButton.gameObject.SetActive( true );
        }
    }

    private void DetailsButtonLogic()
    {
        if( SpriteGridItemController.Selected != null )
        {
            DetailsPage.Instance.LoadSpriteData( SpriteGridItemController.Selected.SpritesData );
            CanvasManager.Instance.SwitchCanvas( CanvasManager.Canvases.Details );
        }
    }

    private async void NextButtonLogic()
    {
        DisableNavigateButtons();

        m_CurrentPage++;
        m_GetSpritesRequest.lastItem = m_SpritesLastItems[ (int) m_CurrentPage - 1 ];

        try
        {
            SpriteData[] spritesData = await SpritesController.GetAllSprites( m_GetSpritesRequest );
            if( spritesData != null && spritesData.Length > 0 )
            {
                LoadSpriteData( spritesData );
                string lastItemId = spritesData[ spritesData.Length - 1 ].id;
                if( m_CurrentPage == m_SpritesLastItems.Count )
                {
                    m_SpritesLastItems.Add( lastItemId );
                }

                // Is there a next page?
                uint currentLimit = m_GetSpritesRequest.limit;
                string currentLastItem = m_GetSpritesRequest.lastItem;
                try
                {
                    m_GetSpritesRequest.limit = 1;
                    m_GetSpritesRequest.lastItem = lastItemId;
                    SpriteData[] spritesData2 = await SpritesController.GetAllSprites( m_GetSpritesRequest );
                    if( spritesData2 != null && spritesData2.Length > 0 )
                    {
                        m_Referencer.NextButton.gameObject.SetActive( true );
                    }
                }
                catch( System.Exception e )
                {
                }
                m_GetSpritesRequest.limit = currentLimit;
                m_GetSpritesRequest.lastItem = currentLastItem;
            }
        }
        catch( System.Exception e )
        {
            ExitSearchLogic( e.Message );
        }
        
        m_Referencer.SearchButton.gameObject.SetActive( true );
        m_Referencer.PreviousButton.gameObject.SetActive( true );
    }

    private void StartSearch()
    {
        
    }

    private void ExitSearchLogic( string errorMessage )
    {
        m_Referencer.ErrorMessage.text = errorMessage;
        m_Referencer.SearchButton.gameObject.SetActive( true );
    }
}
