namespace Capture
{
    /// <summary>
    /// Makes a class to be captured during a session.
    /// </summary>
    public interface ICapturable
    {
        /// <summary>
        /// Export data from a class.
        /// </summary>
        /// <returns>A capture.</returns>
        object GetCapture();
    }
}