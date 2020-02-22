namespace Capture
{
    using UnityEngine;

    public static class TypesExtra
    {
        public static object ToAnonymousType(this Vector3 v)
        {
            return new
            {
                v.x,
                v.y,
                v.z,
            };
        }

        public static object ToAnonymousType(this Vector2 v)
        {
            return new
            {
                v.x,
                v.y,
            };
        }
    }
}