using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidePageReferencer : MonoBehaviour
{

    private void Awake() 
    {
        Referencer.HidePage = this;
    }
}
