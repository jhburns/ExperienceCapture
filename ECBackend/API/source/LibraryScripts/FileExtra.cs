namespace Carter.App.Lib.FileExtra
{
    using System.IO;

    using Carter.App.Hosting;

    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Wraps the filesystem provided by .Net Core.
    /// </summary>
    public class FileExtra
    {
        /// <summary>
        /// Gets the contents of file embedded in this project.
        /// </summary>
        /// <returns>
        /// File contents.
        /// </returns>
        /// <param name="path">Location of the file, from the API.csproj file.</param>
        public static string GetEmbeddedFile(string path)
        {
            var fp = new EmbeddedFileProvider(typeof(Program).Assembly);
            var sourceStream = new StreamReader(fp.GetFileInfo(path).CreateReadStream());
            return sourceStream.ReadToEnd();
        }
    }
}