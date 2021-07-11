using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Static class for converting angles (float) into directions (Vector3) and vice versa.
    /// </summary>
    public static class VectorHelper
    {
        /***************** METHODS ********************/
        public static float GetAngleFromDirection(Vector3 direction)
        {
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        }

        public static Vector3 GetDirectionFromAngle(float angle)
        {
            angle += 90f;
            return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        }
        /**********************************************/
    }
}