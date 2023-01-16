using PuzzleDungeon.Character;
using UnityEngine;

namespace PuzzleDungeon.Tools
{
    public static class Vector3Extension
    {
        public static Vector3 ClampVector3(this Vector3 vector3, Vector3 min, Vector3 max)
        {
            return new Vector3()
            {
                x = Mathf.Clamp(vector3.x, min.x, max.x),
                y = Mathf.Clamp(vector3.y, min.y, max.y),
                z = Mathf.Clamp(vector3.z, min.z, max.z),
            };
        }

        public static float GetVector3Axis(this Vector3 vector3, Axis axis)
        {
            return axis switch
            {
                Axis.X    => vector3.x,
                Axis.Y    => vector3.y,
                Axis.Z    => vector3.z,
                Axis.None => 0,
            };
        }
    }
}