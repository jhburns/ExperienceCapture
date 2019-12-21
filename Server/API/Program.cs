namespace Carter.App.Hosting
{
    using System.Linq;

    using Carter.App.Lib.Authentication;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--passwordGenerate") || args.Contains("-p"))
            {
                PasswordHasher.OutputNew();
                return;
            }

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build();

            host.Run();
        }
    }
}