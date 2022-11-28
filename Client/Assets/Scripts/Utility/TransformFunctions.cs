using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformFunctions
{
    public static void DestroyAllChildren( Transform transform )
    {
        for( int i = transform.childCount - 1; i >= 0; i-- )
        {
            GameObject currentChild = transform.GetChild( i ).gameObject;
            currentChild.SetActive( false );
            currentChild.transform.SetParent( null );
            GameObject.Destroy( currentChild );
        }
    }

    public static void DestroyAllChildren( GameObject gameObject )
    {
        DestroyAllChildren( gameObject.transform );
    }
}
