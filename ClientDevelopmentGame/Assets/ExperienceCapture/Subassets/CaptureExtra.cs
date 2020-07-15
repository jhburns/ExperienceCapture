namespace Capture
{
    using UnityEngine;

    /// <summary>
    /// Helpers to reduce repetition when capturing data.
    /// </summary>
    public static class TypesExtra
    {
        /// <summary>
        /// Convert a Vector3 to an anonymous type.
        /// </summary>
        /// <param name="v">A vector to be converted.</param>
        /// <returns>An anonymous type.</returns>
        public static object ToAnonymousType(this Vector3 v)
        {
            return new
            {
                v.x,
                v.y,
                v.z,
            };
        }

        /// <summary>
        /// Convert a Vector2 to an anonymous type.
        /// </summary>
        /// <param name="v">A vector to be converted.</param>
        /// <returns>An anonymous type.</returns>
        public static object ToAnonymousType(this Vector2 v)
        {
            return new
            {
                v.x,
                v.y,
            };
        }
    }

    /// <summary>
    /// Helpers that calculate vision data.
    /// </summary>
    public static class VisionExtra
    {
        /// <summary>
        /// Get an angle from a game object to another.
        /// </summary>
        /// <param name="origin">A starting object.</param>
        /// <param name="destination">An ending object.</param>
        /// <param name="isDraw">When true, draw a line representing this calculation.</param>
        /// <returns></returns>
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

    /// <summary>
    /// Programmatic way to configure capturing.
    /// Not every configuration is exposed yet.
    /// </summary>
    public class CaptureConfig
    {
        private static KeyCode cleanupKey = KeyCode.Q;

        // Basic getters/setters because default values means no { get; set; }
        /// <summary>
        /// Set the override key.
        /// </summary>
        /// <param name="key">Am override key.</param>
        public static void OverrideCleanupKey(KeyCode key)
        {
            cleanupKey = key;
        }

        /// <summary>
        /// Get the override key.
        /// </summary>
        /// <returns>The override key.</returns>
        public static KeyCode GetCleanupKey()
        {
            return cleanupKey;
        }
    }
}