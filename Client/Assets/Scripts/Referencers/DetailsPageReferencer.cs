using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailsPageReferencer : MonoBehaviour
{
    public Button BackButton;
    public Image SpriteContent;
    public TextMeshProUGUI Id;
    public TextMeshProUGUI Author;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI DateCreated;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI GlobalRating;
    public TextMeshProUGUI Tags;
    public TextMeshProUGUI ErrorMessage;
    public Button RateButton;
    public Button DownloadButton;
    public Button[] StarButtons;
    public Image[] StarImages;


    private void Awake() 
    {
        Referencer.DetailsPage = this;
    }
}
