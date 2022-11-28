using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteGridItemController : MonoBehaviour
{
    // Fields
    [SerializeField] private Button m_Button;
    [SerializeField] private Image[] SelectedImages;
    [SerializeField] private Image m_ContentImage;

    // Variables
    public SpriteData SpritesData { get; private set; }
    private bool m_IsActive = false;
    public static SpriteGridItemController Selected { get; private set; }

    private void Start() 
    {
        m_Button.onClick.AddListener( ClickLogic );
    }

    public void SetSpritesData( SpriteData spritesData )
    {
        SpritesData = spritesData;
        m_ContentImage.sprite = ConvertManager.GetSpriteFromString( SpritesData.content, SpritesData.dimensionX, SpritesData.dimensionY );
        m_ContentImage.preserveAspect = true;
    }

    private void ClickLogic()
    {
        m_Button.interactable = false;

        if( Selected == null )
        {
            ChangeState( true );
            Selected = this;
            Referencer.BrowsePage.DetailsButton.gameObject.SetActive( true );
        }
        else if( Selected == this )
        {
            ChangeState( false );
            Selected = null;
            Referencer.BrowsePage.DetailsButton.gameObject.SetActive( false );
        }
        else
        {
            Selected.ChangeState( false );
            ChangeState( true );
            Selected = this;
            Referencer.BrowsePage.DetailsButton.gameObject.SetActive( true );
        }

        m_Button.interactable = true;
    }

    private void ChangeState( bool isActive )
    {
        m_IsActive = isActive;
        float newAlpha = m_IsActive ? 1f : 0f;
        for(int i = 0; i < SelectedImages.Length; i++)
        {
            SelectedImages[i].color = new Color( SelectedImages[i].color.r, SelectedImages[i].color.g, SelectedImages[i].color.b, newAlpha );
        }
    }
}
