using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxGizmoBox : MonoBehaviour
{

   public enum GizmoType {
      Sphere,
      Axis
   }

   public GizmoType gizmoType = GizmoType.Sphere;
   public float size = 0.1f;
   public Color color = Color.blue;

   /// <summary>
   /// Start is called on the frame when a script is enabled just before
   /// any of the Update methods is called the first time.
   /// </summary>
   void Start () {

   }
   private void OnDrawGizmos () {
      if (enabled) {
         if(gizmoType == GizmoType.Sphere) {
            Gizmos.color = color;
            Gizmos.DrawSphere (transform.position, size);
         } else if(gizmoType == GizmoType.Axis) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * size);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * size);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * size);
         }
      }
   }
}
