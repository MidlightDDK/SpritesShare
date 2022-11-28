using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BackgroundController : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    private float m_EdgePosition;
    public float ScrollDownSpeed;
    [SerializeField]
    private Sprite[] m_Sprites;

    void Awake()
    {
        // Get the required components
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        // Check if everything was imported
        Assert.IsNotNull(m_SpriteRenderer, "Button component is missing...");

        // Initial values
        RefreshColor();
        m_EdgePosition = m_SpriteRenderer.size.y / 4;
        m_SpriteRenderer.sprite = m_Sprites[Random.Range(0, m_Sprites.Length)];
    }

    private void OnValidate()
    {
        Assert.IsNotNull(m_Sprites, "OnValidateError: No sprites provided...");
        Assert.AreNotEqual(m_Sprites.Length, 0, "OnValidateError: No sprites provided...");
    }

    private void Update()
    {
        // Background goes down
        transform.position += Vector3.down * ScrollDownSpeed * Time.deltaTime;

        // Teleport up if it's too down
        if (transform.position.y < -m_EdgePosition)
        {
            transform.position += Vector3.up * 2 * m_EdgePosition;
        }

        // Refresh Color
        RefreshColor();
    }

    private void RefreshColor()
    {
        m_SpriteRenderer.color = ColorManager.Instance.ColorSecondary;
    }
}
