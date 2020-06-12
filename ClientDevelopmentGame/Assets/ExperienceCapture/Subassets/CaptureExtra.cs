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

    public static class VisionExtra
    {
        public static float AngleWith(this MonoBehaviour origin, MonoBehaviour destination, bool isDraw = false)
        {
            // First, make a vector that goes in the direction the origin is looking at
            Vector3 facing = origin.transform.forward;
            Vector3 originPosition = origin.transform.position;

            if (isDraw)
            {
                Debug.DrawRay(originPosition, facing * 800, Color.red);
            }

            // Use destination position to get a vector from the origin to the destination
            Vector3 destinationPos = destination.transform.position;
            Vector3 originToDestination = destinationPos - originPosition;

            if (isDraw)
            {
                Debug.DrawRay(originPosition, originToDestination, Color.blue);
            }

            // Calculate the angle between these vectors
            // Which will give origin angle relative to destination
            return Vector3.Angle(originToDestination, facing);
        }
    }

    public class CaptureConfig
    {
        private static KeyCode cleanupKey = KeyCode.Q;

        // Basic getters/setters because default values means no { get; set; }
        public static void OverrideCleanupKey(KeyCode key)
        {
            cleanupKey = key;
        }

        public static KeyCode GetCleanupKey()
        {
            return cleanupKey;
        }
    }
}