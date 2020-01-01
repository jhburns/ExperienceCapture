namespace Exporter.App.Main
{
    using System;

    using Exporter.App.ExportHandler;

    public class Entry
    {
        public static void Main(string[] args)
        {
            try
            {
                ExportHandler.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}