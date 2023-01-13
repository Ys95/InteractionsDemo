using System;
//using Sirenix.OdinInspector;
using UnityEngine;

namespace Tide
{
    public class DrawGizmo : MonoBehaviour
    {
        enum GizmoType
        {
            Sphere,
            Cube
        }

        [SerializeField] private GizmoType gizmoType  = GizmoType.Sphere;
        [SerializeField] private Color     color      = Color.yellow;
       // [ShowIf("gizmoType", GizmoType.Sphere)]
        [SerializeField] private float radius = 0.1f;
      //  [ShowIf("gizmoType", GizmoType.Cube)]
        [SerializeField] private Vector3 size = new Vector3(0.1f, 0.1f, 0.1f);

        private void OnDrawGizmos()
        {
            var rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(Vector3.zero), transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.color  = color;

            if (gizmoType == GizmoType.Sphere)
            {
                Gizmos.DrawSphere(Vector3.zero, radius);
            }
            else
            {
                Gizmos.DrawCube(Vector3.zero, size);
            }
        }
    }
}