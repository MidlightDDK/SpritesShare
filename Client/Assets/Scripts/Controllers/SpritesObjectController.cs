using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesObjectController : MonoBehaviour
{
    private float m_MovementSpeed;
    private const float m_MinMovementSpeed = 1f;
    private const float m_MaxMovementSpeed = 5f;
    private const float m_SafePrecision = 1f;

    private Transform m_Transform;
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    private float m_SafeAreaX;
    private float m_SafeAreaY;
    public static SpriteData[] GetAllSpritesResponse = null;

    private void Awake() {
        m_Transform = transform;
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        Vector3 safeArea = Camera.main.ScreenToWorldPoint( new Vector3( Screen.safeArea.xMax, Screen.safeArea.yMax )  ) ;
        m_SafeAreaX = safeArea.x;
        m_SafeAreaY = safeArea.y;
    }

    private void Start()
    {
        m_MovementSpeed = Random.Range( m_MinMovementSpeed, m_MaxMovementSpeed );
        m_Animator.SetFloat( "Speed", m_MovementSpeed );

        Reset();
    }

    private void Update()
    {
        // Move
        m_Transform.Translate( Vector3.down * m_MovementSpeed * Time.deltaTime );

        // Reset logic
        if( m_Transform.position.y < -m_SafeAreaY - m_SafePrecision )
        {
            Reset();
        }
    }

    private void Reset()
    {
        // Movement speed
        m_MovementSpeed = Random.Range( m_MinMovementSpeed, m_MaxMovementSpeed );
        m_Animator.SetFloat( "Speed", m_MovementSpeed );

        // Starting pos
        m_Transform.position = new Vector3( Random.Range( -m_SafeAreaX + m_SafePrecision, m_SafeAreaX - m_SafePrecision ), m_SafeAreaY + m_SafePrecision, 0 );

        // Sprite used
        if( GetAllSpritesResponse != null && GetAllSpritesResponse.Length > 0 )
        {
            int randomIndex = Random.Range( 0, GetAllSpritesResponse.Length );
            m_SpriteRenderer.sprite = ConvertManager.GetSpriteFromString( GetAllSpritesResponse[randomIndex].content, GetAllSpritesResponse[randomIndex].dimensionX, GetAllSpritesResponse[randomIndex].dimensionY );
        }

        // Color used
        m_SpriteRenderer.color = new Color( Random.Range( 0f, 1f ), Random.Range( 0f, 1f ), Random.Range( 0f, 1f ) );
    }
}
