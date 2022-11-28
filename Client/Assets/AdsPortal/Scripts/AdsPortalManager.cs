using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsPortalManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ColorManager.Instance.Update();
    }

    public void LaunchLink(string link)
    {
        Application.OpenURL(link);
    }
}
