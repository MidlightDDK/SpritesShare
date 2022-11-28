using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class AdsButton : MonoBehaviour
{
    [Header("Required Serialized Fields")]
    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private TextMeshProUGUI m_TextAdditional;

    // Internal values
    private bool m_HasBeenClickedOnce = false;
    private bool m_IsDataValid = true;

    // Ads links
    [System.Serializable]
    public class AdsUrl
    {
        public string Url;
        public float ChanceOfAppearing;
    }
    [Header("Settings")]
    [SerializeField] private AdsUrl[] m_AdsUrls;
    private float m_AdsChancePrecision = 0.001f;
    private float[] m_CalculatedChances;

    // Game Starting Scene
    [SerializeField] private string m_StartingScene = "";

    // Displayed Text
    [SerializeField] private string WatchAdText = "";
    [SerializeField] private string PlayGameText = "";


    private void Awake()
    {
        // Check if everything was imported
        Assert.IsNotNull(m_Button, "Button component is missing...");
        Assert.IsNotNull(m_Text, "TextMeshProUGUI is missing in child...");

        // If it's fine, we add the right logic
        m_Button.onClick.AddListener(ClickLogic);
        m_Text.text = WatchAdText;
    }


    private void OnValidate()
    {
        // Starting scene empty error
        if (m_StartingScene == "")
        {
            Debug.LogError("OnValidateError: StartingScene not provided...");
            m_IsDataValid = false;
            return;
        }

        // WathAd Text empty error
        if (WatchAdText == "")
        {
            Debug.LogError("OnValidateError: WatchAdText not provided...");
            m_IsDataValid = false;
            return;
        }

        // PlayGame Text empty error
        if (PlayGameText == "")
        {
            Debug.LogError("OnValidateError: PlayGameText not provided...");
            m_IsDataValid = false;
            return;
        }

        // No Ads link error
        if (m_AdsUrls == null || m_AdsUrls.Length == 0)
        {
            Debug.LogError("OnValidateError: At least one Ad Link should be provided...");
            m_IsDataValid = false;
            return;
        }

        // Wrong sum of chance error
        float sumPercentage = 0f;
        for (int i = 0; i < m_AdsUrls.Length; i++)
        {
            sumPercentage += m_AdsUrls[i].ChanceOfAppearing;
        }

        if (m_AdsUrls.Length > 1 && Mathf.Abs(sumPercentage - 100f) > m_AdsChancePrecision)
        {
            Debug.LogError("OnValidateError: Sum of Chance of Appearing should be 100!");
            m_IsDataValid = false;
            return;
        }

        // Valid state
        m_CalculatedChances = new float[m_AdsUrls.Length];
        for(int i = 0; i < m_CalculatedChances.Length; i++)
        {
            m_CalculatedChances[i] = m_AdsUrls[i].ChanceOfAppearing;
            if(i > 0)
            {
                m_CalculatedChances[i] += m_CalculatedChances[i - 1];
            }
        }
        m_IsDataValid = true;
    }

    private void ClickLogic()
    {
        // If Data is invalid, there is no further processing
        if (!m_IsDataValid)
        {
            return;
        }

        // If it is valid, we use the right function corresponding to the state of the button
        if (!m_HasBeenClickedOnce)
        {
            OpenAds();
            m_HasBeenClickedOnce = true;
            m_Text.text = PlayGameText;
            FindObjectOfType<BlipController>().HasWatched();
            Destroy(m_TextAdditional.gameObject);
        }
        else
        {
            LaunchGame();
        }
    }

    private void OpenAds()
    {
        if(m_AdsUrls.Length == 1)
        {
            Application.OpenURL(m_AdsUrls[0].Url);
        }
        else
        {
            float randomNumber = Random.Range(0f, 100f);
            for (int i = 0; i < m_CalculatedChances.Length; i++)
            {
                if (randomNumber <= m_CalculatedChances[i])
                {
                    Application.OpenURL(m_AdsUrls[i].Url);
                    return;
                }
            }
        }
    }

    private void LaunchGame()
    {
        SceneManager.LoadSceneAsync(m_StartingScene);
    }
}
