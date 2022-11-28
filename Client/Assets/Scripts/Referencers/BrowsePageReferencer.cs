using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrowsePageReferencer : MonoBehaviour
{
    public Button BackButton;
    public Button SearchButton;
    public Button PreviousButton;
    public Button DetailsButton;
    public Button NextButton;
    public Toggle AscendingField;
    public TMP_InputField TagsField;
    public TMP_InputField AuthorField;
    public TMP_InputField NameField;
    public TMP_Dropdown DimensionDropDown;
    public TMP_InputField DimensionMinField;
    public TMP_InputField DimensionMaxField;
    public TMP_InputField RatingMinField;
    public TMP_InputField RatingMaxField;
    public RectTransform GridContainer;
    public TextMeshProUGUI ErrorMessage;

    private void Awake() 
    {
        Referencer.BrowsePage = this;
    }
}
