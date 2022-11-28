using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpritesSender", menuName = "SpritesShares/SpritesSender", order = 0)]
public class SpritesSender : ScriptableObject 
{
    public string spritesAuthor;
    public Texture2D spritesContent;
    public string spritesDescription;
    public string spritesName;
    public string[] spritesTags = new string[0];

    public bool SendButton = false;
    private bool m_IsProcessing = false;
    private async void OnValidate() 
    {
        if( SendButton )
        {
            SendButton = false;

            if( m_IsProcessing )
            {
                throw new System.Exception( "Already processing the data." );
            }
            m_IsProcessing = true;
            Debug.Log( "Processing request..." );

            // Errors
            if( string.IsNullOrWhiteSpace( spritesAuthor ) )
            {
                throw new System.Exception( "spritesAuthor not provided." );
            }
            else if( spritesContent == null )
            {
                throw new System.Exception( "spritesContent not provided." );
            }
            else if( string.IsNullOrWhiteSpace( spritesDescription ) )
            {
                throw new System.Exception( "spritesDescription not provided." );
            }
            else if( string.IsNullOrWhiteSpace( spritesName ) )
            {
                throw new System.Exception( "spritesName not provided." );
            }

            // Send logic
            await SpritesController.AddSprites( new AddSpriteRequest(){
                author = spritesAuthor,
                content = ConvertManager.GetStringFromTexture( spritesContent ),
                description = spritesDescription,
                dimensionX = (uint) spritesContent.width,
                dimensionY = (uint) spritesContent.height,
                name = spritesName,
                tags = spritesTags,
            } );
            m_IsProcessing = false;
        }
    }
}
