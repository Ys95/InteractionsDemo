using UnityEngine;

namespace PuzzleDungeon.Tools
{
    public class DrawCollider : MonoBehaviour
    {
        [SerializeField] private bool     draw = true;
        [SerializeField] private Collider detect;
        [SerializeField] private bool     drawDuringPlaytime;

        [Space]
        [SerializeField] private Color color;

        public bool Draw
        {
            get => draw;
            set => draw = value;
        }

        public Collider Detect
        {
            get => detect;
            set => detect = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public bool DrawDuringPlaytime
        {
            get => drawDuringPlaytime;
            set => drawDuringPlaytime = value;
        }

        private void OnDrawGizmos()
        {
            if (!draw) return;

            if (detect == null)
            {
                detect = GetComponent<Collider>();
            }

            if (!drawDuringPlaytime && Application.isPlaying) return;

            Gizmos.color = color;
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(detect.gameObject.transform.position, detect.gameObject.transform.rotation, detect.gameObject.transform.lossyScale);
            Gizmos.matrix = matrix4X4;

            if (detect.GetType() == typeof(BoxCollider))
            {
                var boxCollider = detect as BoxCollider;
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            }
            else if (detect.GetType() == typeof(SphereCollider))
            {
                var sphereCollider = detect as SphereCollider;
                Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
            }
            else if (detect.GetType() == typeof(MeshCollider))
            {
                var meshCollider = detect as MeshCollider;
                Gizmos.DrawMesh(meshCollider.sharedMesh);
            }
        }
    }
}