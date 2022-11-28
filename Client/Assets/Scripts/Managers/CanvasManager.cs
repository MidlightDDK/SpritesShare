using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager
{
    private static CanvasManager m_Instance;
    public static CanvasManager Instance
    {
        get
        {
            if( m_Instance == null )
            {
                m_Instance = new CanvasManager();
            }
            return m_Instance;
        }
    }
    private bool m_IsInit = false;
    public enum Canvases
    {
        Browse,
        Details,
        Hide,
        Upload,
        Welcome,
    }
    private Canvases m_CurrentCanvas = Canvases.Welcome;
    private GameObject[] m_CanvasArray = null;

    private CanvasManager()
    {
        // Get reference to each canvas
        GameObject[] canvasArray = {
            Referencer.BrowsePage.gameObject,
            Referencer.DetailsPage.gameObject,
            Referencer.HidePage.gameObject,
            Referencer.UploadPage.gameObject,
            Referencer.WelcomePage.gameObject,
        };
        m_CanvasArray = canvasArray;

        DisableAllCanvas();
    }

    public void Init()
    {
        if( m_IsInit )
        {
            return;
        }
        m_IsInit = true;
        SwitchCanvas( m_CurrentCanvas );
    }

    private void DisableAllCanvas()
    {
        for( int i = 0; i < m_CanvasArray.Length; i++ )
        {
            if( m_CanvasArray[i].activeInHierarchy )
            {
                m_CanvasArray[i].SetActive( false );
            }
        }
    }

    public void SwitchCanvas( Canvases targetCanvas )
    {
        m_CanvasArray[ (int) m_CurrentCanvas ].SetActive( false );
        m_CanvasArray[ (int) targetCanvas ].SetActive( true );
        m_CurrentCanvas = targetCanvas;
    }
    
}
