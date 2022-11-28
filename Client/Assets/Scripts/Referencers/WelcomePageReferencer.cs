using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WelcomePageReferencer : MonoBehaviour
{
    public Button UploadButton;
    public Button BrowseButton;
    public Button ToggleMusicButton;
    public Button ExitButton;
    public TextMeshProUGUI ToggleMusicText;
    public AudioSource MusicPlayer;

    private void Awake() 
    {
        Referencer.WelcomePage = this;
    }
}
