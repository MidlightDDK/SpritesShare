using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColor : MonoBehaviour
{
    private Camera m_Camera;
	private Color m_TargetColor;
	[SerializeField] [Range( 0f, 0.09f )] private float m_ColorIncrement = 0.001f;

    private void Awake() 
    {
        m_Camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
		MakeNewColorTarget();
    }

    // Update is called once per frame
    void Update()
    {
		// If we reached the desired color
		if( Mathf.Abs( m_Camera.backgroundColor.r - m_TargetColor.r ) < 0.1f && Mathf.Abs( m_Camera.backgroundColor.g - m_TargetColor.g ) < 0.1f && Mathf.Abs( m_Camera.backgroundColor.b - m_TargetColor.b ) < 0.1f )
		{
			MakeNewColorTarget();
		}
        
		// Advance towards next color
		Color nextColor = new Color();
		nextColor.r = m_Camera.backgroundColor.r;
		nextColor.g = m_Camera.backgroundColor.g;
		nextColor.b = m_Camera.backgroundColor.b;

		// Update each color component
		UpdateColorComponent( ref nextColor.r, m_TargetColor.r );
		UpdateColorComponent( ref nextColor.g, m_TargetColor.g );
		UpdateColorComponent( ref nextColor.b, m_TargetColor.b );

		// Update the actual color
		m_Camera.backgroundColor = nextColor;
    }

	void MakeNewColorTarget()
	{
		m_TargetColor = new Color( Random.Range( 0f, 1f ), Random.Range( 0f, 1f ), Random.Range( 0f, 1f ) );
	}

	private void UpdateColorComponent( ref float actualColorComponent, float targetColorComponent )
	{
		if( Mathf.Abs( actualColorComponent - targetColorComponent ) > 0.1f )
		{
			if( actualColorComponent < targetColorComponent )
				actualColorComponent += m_ColorIncrement;
			else
				actualColorComponent -= m_ColorIncrement;
		}
	}
}
