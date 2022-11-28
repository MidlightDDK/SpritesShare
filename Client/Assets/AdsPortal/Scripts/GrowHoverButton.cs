using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class GrowHoverButton : MonoBehaviour
{
    [Header("Required Serialized Fields")]
    [SerializeField] private EventTrigger m_EventTrigger;

    // Internal values
    private bool m_IsHovered = false;
    private bool m_IsChanging = false;

    [Header("Hover Settings")]
    [SerializeField] private float m_NormalScale = 1f;
    [SerializeField] private float m_HoverScale = 1.25f;
    [SerializeField] private float m_TransitionTime = 0.25f;
    private float m_IncrementPerFrame;

    private void Awake()
    {
        // Check if everything was imported
        Assert.IsNotNull(m_EventTrigger, "EventTrigger component is missing...");

        // Event triggers
        EventTrigger.Entry entryOnPointerEnter = new EventTrigger.Entry();
        entryOnPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryOnPointerEnter.callback.AddListener((data) => { OnPointerEnterDeletegate((PointerEventData)data); });

        EventTrigger.Entry entryOnPointerExit = new EventTrigger.Entry();
        entryOnPointerExit.eventID = EventTriggerType.PointerExit;
        entryOnPointerExit.callback.AddListener((data) => { OnPointerExitDeletegate((PointerEventData)data); });

        m_EventTrigger.triggers.Add(entryOnPointerEnter);
        m_EventTrigger.triggers.Add(entryOnPointerExit);

        // Calculate Increment per seconds needed
        m_IncrementPerFrame = ((m_HoverScale - m_NormalScale) / m_TransitionTime);
    }

    private void OnPointerEnterDeletegate(PointerEventData data)
    {
        m_IsChanging = true;
        m_IsHovered = true;
    }

    private void OnPointerExitDeletegate(PointerEventData data)
    {
        m_IsChanging = true;
        m_IsHovered = false;
    }

    private void Update()
    {
        if(m_IsChanging)
        {
            if (m_IsHovered)
            {
                Vector3 newScale = transform.localScale;

                if (transform.localScale.x < m_HoverScale)
                {
                    newScale.x = Mathf.Clamp(newScale.x + (m_IncrementPerFrame * Time.deltaTime), m_NormalScale, m_HoverScale);
                }
                if (transform.localScale.y < m_HoverScale)
                {
                    newScale.y = Mathf.Clamp(newScale.y + (m_IncrementPerFrame * Time.deltaTime), m_NormalScale, m_HoverScale);
                }

                if(newScale == transform.localScale)
                {
                    m_IsChanging = false;
                }
                else
                {
                    transform.localScale = newScale;
                }
            }
            else
            {
                Vector3 newScale = transform.localScale;

                if (transform.localScale.x > m_NormalScale)
                {
                    newScale.x = Mathf.Clamp(newScale.x - (m_IncrementPerFrame * Time.deltaTime), m_NormalScale, m_HoverScale);
                }
                if (transform.localScale.y > m_NormalScale)
                {
                    newScale.y = Mathf.Clamp(newScale.y - (m_IncrementPerFrame * Time.deltaTime), m_NormalScale, m_HoverScale);
                }

                if (newScale == transform.localScale)
                {
                    m_IsChanging = false;
                }
                else
                {
                    transform.localScale = newScale;
                }
            }
        }
        
    }
}
