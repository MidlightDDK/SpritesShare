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
    
    // Functions
    private void Start()
    {
        CanvasManager.Instance.Init();
        WelcomePage.Instance.Init();
        UploadPage.Instance.Init();
        DetailsPage.Instance.Init();
        BrowsePage.Instance.Init();
    }
}
