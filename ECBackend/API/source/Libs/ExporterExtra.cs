namespace Carter.App.Libs.ExporterExtra
{
    using System.Threading;

    /// <summary>
    /// Wraps System.Threading with an interface.
    /// </summary>
    public interface IThreadExtra
    {
        /// <summary>
        /// Creates an starts a thread.
        /// </summary>
        /// <param name="method">A method to run in a new thread.</param>
        /// <param name="parameter">The parameters for the thread method. Pass nothing for parameterless methods.</param>
        void Run(ParameterizedThreadStart method, object parameter = null);
    }
}