﻿namespace Nancy.App.Hosting.Kestrel
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    public class API
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}