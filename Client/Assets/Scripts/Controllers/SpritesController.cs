using System.Net;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public static class SpritesController
{
    private const bool m_IsDebug = true;

    public static async Task<SpriteData[]> GetAllSprites( GetAllSpritesRequest spritesRequest = null )
    {
        string query = "?";
        if( spritesRequest != null )
        {
            if( spritesRequest.limit > 0 )
            {
                query += "&limit=" + spritesRequest.limit;
            }
            if( spritesRequest.asc )
            {
                query += "&asc=true";
            }
            if( !string.IsNullOrWhiteSpace( spritesRequest.tags ) )
            {
                query += "&tags=" + spritesRequest.tags;
            }
            if( !string.IsNullOrWhiteSpace( spritesRequest.author ) )
            {
                query += "&author=" + spritesRequest.author;
            }
            if( !string.IsNullOrWhiteSpace( spritesRequest.name ) )
            {
                query += "&name=" + spritesRequest.name;
            }
            if( spritesRequest.dimensionXMax > 0 )
            {
                query += "&dimensionXMax=" + spritesRequest.dimensionXMax;
            }
            if( spritesRequest.dimensionXMin > 0 )
            {
                query += "&dimensionXMin=" + spritesRequest.dimensionXMin;
            }
            if( spritesRequest.dimensionYMax > 0 )
            {
                query += "&dimensionYMax=" + spritesRequest.dimensionYMax;
            }
            if( spritesRequest.dimensionYMin > 0 )
            {
                query += "&dimensionYMin=" + spritesRequest.dimensionYMin;
            }
            if( spritesRequest.ratingMax > 0 )
            {
                query += "&ratingMax=" + spritesRequest.ratingMax;
            }
            if( spritesRequest.ratingMin > 0 )
            {
                query += "&ratingMin=" + spritesRequest.ratingMin;
            }
            if( !string.IsNullOrWhiteSpace( spritesRequest.lastItem ) )
            {
                query += "&lastItem=" + spritesRequest.lastItem;
            }
        }
        query = query.Replace( "?&", "?" );

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create( "https://sprites-share-api-rwvkn4ky5a-lm.a.run.app/sprites" + query );
        Debug.Log( "https://sprites-share-api-rwvkn4ky5a-lm.a.run.app/sprites" + query );

        HttpWebResponse response = (HttpWebResponse)( await request.GetResponseAsync() );
        StreamReader reader = new StreamReader( response.GetResponseStream() );
        string jsonResponse = reader.ReadToEnd();
        if( m_IsDebug )
        {
            Debug.Log( jsonResponse );
        }
        SpriteData[] result = JsonConvert.DeserializeObject<SpriteData[]>( jsonResponse );
        return result;
    }

    public static async Task<SpriteData> GetSprites( string spritesId )
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create( "https://sprites-share-api-rwvkn4ky5a-lm.a.run.app/sprites/" + spritesId );

        HttpWebResponse response = (HttpWebResponse)( await request.GetResponseAsync() );
        StreamReader reader = new StreamReader( response.GetResponseStream() );
        string jsonResponse = reader.ReadToEnd();
        if( m_IsDebug )
        {
            Debug.Log( jsonResponse );
        }
        SpriteData result = JsonConvert.DeserializeObject<SpriteData>( jsonResponse );
        return result;
    }

    public static async Task<ApiResponse> AddSprites( AddSpriteRequest apiRequest )
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create( "https://sprites-share-api-rwvkn4ky5a-lm.a.run.app/sprites" );
        request.ContentType = "application/json";
        request.Method = "POST";
        using ( var streamWriter = new StreamWriter( request.GetRequestStream() ) )
        {
            streamWriter.Write( JsonConvert.SerializeObject( apiRequest ) );
        }
        
        HttpWebResponse response = (HttpWebResponse)( await request.GetResponseAsync() );
        StreamReader reader = new StreamReader( response.GetResponseStream() );
        string jsonResponse = reader.ReadToEnd();
        if( m_IsDebug )
        {
            Debug.Log( jsonResponse );
        }
        ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>( jsonResponse );
        return apiResponse;
    }

    public static async Task<ApiResponse> RateSprites( string spritesId, RateSpriteRequest apiRequest )
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create( "https://sprites-share-api-rwvkn4ky5a-lm.a.run.app/sprites/" + spritesId );
        request.ContentType = "application/json";
        request.Method = "POST";
        using ( var streamWriter = new StreamWriter( request.GetRequestStream() ) )
        {
            streamWriter.Write( JsonConvert.SerializeObject( apiRequest ) );
        }
        
        HttpWebResponse response = (HttpWebResponse)( await request.GetResponseAsync() );
        StreamReader reader = new StreamReader( response.GetResponseStream() );
        string jsonResponse = reader.ReadToEnd();
        if( m_IsDebug )
        {
            Debug.Log( jsonResponse );
        }
        ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>( jsonResponse );
        return apiResponse;
    }
}
