using System.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public static class SpritesController
{
    private const bool m_IsDebug = true;

    public static async Task<Sprites[]> GetAllSprites()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create( "https://sprites-share-api-rwvkn4ky5a-lm.a.run.app/sprites" );

        HttpWebResponse response = (HttpWebResponse)( await request.GetResponseAsync() );
        StreamReader reader = new StreamReader( response.GetResponseStream() );
        string jsonResponse = reader.ReadToEnd();
        if( m_IsDebug )
        {
            Debug.Log( jsonResponse );
        }
        Sprites[] result = JsonConvert.DeserializeObject<Sprites[]>( jsonResponse );
        return result;
    }

    public static async Task<Sprites> GetSprites( string spritesId )
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create( "https://sprites-share-api-rwvkn4ky5a-lm.a.run.app/sprites/" + spritesId );

        HttpWebResponse response = (HttpWebResponse)( await request.GetResponseAsync() );
        StreamReader reader = new StreamReader( response.GetResponseStream() );
        string jsonResponse = reader.ReadToEnd();
        if( m_IsDebug )
        {
            Debug.Log( jsonResponse );
        }
        Sprites result = JsonConvert.DeserializeObject<Sprites>( jsonResponse );
        return result;
    }

    public static async Task<ApiResponse> AddSprites( AddSpritesRequest apiRequest )
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

    public static async Task<ApiResponse> RateSprites( string spritesId, RateSpritesRequest apiRequest )
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
