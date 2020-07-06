namespace Carter.App.Lib.ExporterExtra
{
    using System.Threading;

    public interface IThreadExtra
    {
        void Run(ParameterizedThreadStart method, object parameter = null);
    }
}