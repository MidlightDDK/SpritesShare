using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UploadPageReferencer : MonoBehaviour
{
    public Button BackButton;
    public Image SpriteContent;
    public TMP_InputField Author;
    public TMP_InputField Name;
    public TMP_InputField Description;
    public TMP_InputField Tags;
    public TextMeshProUGUI ErrorMessage;
    public Button ImportButton;
    public Button ShareButton;

    private void Awake() 
    {
        Referencer.UploadPage = this;
    }
}
