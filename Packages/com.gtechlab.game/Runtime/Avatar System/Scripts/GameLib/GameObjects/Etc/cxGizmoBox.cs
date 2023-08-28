using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxGizmoBox : MonoBehaviour
{

    public float size = 0.1f;
    public Color color = Color.blue;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
    }
   private void OnDrawGizmos() {
    if(enabled) {
       Gizmos.color = color;
       Gizmos.DrawSphere(transform.position, size);
    }
   }
}
