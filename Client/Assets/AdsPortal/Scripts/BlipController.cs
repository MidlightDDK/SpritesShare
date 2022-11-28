using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class BlipController : MonoBehaviour
{
    private Image m_Image;
    [SerializeField]
    private Sprite[] m_SpritesNotWatched;
    [SerializeField]
    private Sprite[] m_SpritesWatched;
    [SerializeField]
    private float m_ChangeSpriteTimer = 0f;
    private bool m_Watched = false;

    void Awake()
    {
        // Get the required components
        m_Image = GetComponent<Image>();

        // Check if everything was imported
        Assert.IsNotNull(m_Image, "Button component is missing...");

        // Pick the right color
        RefreshColor();

        // Switch Sprite routine
        if (m_ChangeSpriteTimer > 0f)
        {
            InvokeRepeating("SwitchSprite", 0f, m_ChangeSpriteTimer);
        }
    }

    private void OnValidate()
    {
        Assert.IsNotNull(m_SpritesNotWatched, "OnValidateError: No sprites for NotWatched provided...");
        Assert.AreNotEqual(m_SpritesNotWatched.Length, 0, "OnValidateError: No sprites for NotWatched provided...");
        Assert.IsNotNull(m_SpritesWatched, "OnValidateError: No sprites for Watched provided...");
        Assert.AreNotEqual(m_SpritesWatched.Length, 0, "OnValidateError: No sprites for Watched provided...");
    }

    private void SwitchSprite()
    {
        if(m_Watched)
        {
            m_Image.sprite = m_SpritesWatched[Random.Range(0, m_SpritesWatched.Length)];
        }
        else
        {
            m_Image.sprite = m_SpritesNotWatched[Random.Range(0, m_SpritesNotWatched.Length)];
        }
    }

    public void HasWatched()
    {
        m_Watched = true;
    }

    private void RefreshColor()
    {
        m_Image.color = ColorManager.Instance.ColorPrimary;
    }

    private void Update()
    {
        RefreshColor();
    }
}
