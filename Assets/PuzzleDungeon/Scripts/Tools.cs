using UnityEngine;

namespace Tide
{
    public static class Tools
    {
        public static bool LayerInLayerMask(LayerMask layerMask, int layer) => layerMask == (layerMask | (1 << layer));

        public static Vector3 ClampVector3(Vector3 vector3, Vector3 min, Vector3 max)
        {
            return new Vector3()
            {
                x = Mathf.Clamp(vector3.x, min.x, max.x),
                y = Mathf.Clamp(vector3.y, min.y, max.y),
                z = Mathf.Clamp(vector3.z, min.z, max.z),
            };
        }
    }
}