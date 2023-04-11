using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxGizmoBox : MonoBehaviour
{
    public float size = 0.1f;
    public Color color = Color.blue;

   private void OnDrawGizmos() {
       Gizmos.color = color;
       Gizmos.DrawSphere(transform.position, size);

   }
}
