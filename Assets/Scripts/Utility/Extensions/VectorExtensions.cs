using UnityEngine;

namespace Utility.Extensions
{
    public static class VectorExtensions
    {
        public static float GetAngle(this Vector2 vector)
        {
            return GetAngle((Vector3) vector);
        }

        public static float GetAngle(this Vector3 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg - 90f;
        }

        public static Vector2 GetDirectionVector(this float angle)
        {
            angle = (angle + 90f) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}