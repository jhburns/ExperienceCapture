namespace Carter.App.Hosting
{
    using System;
    using System.Linq;

    using Carter.App.Lib.Authentication;
    using Carter.App.Lib.Environment;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--passwordGenerate") || args.Contains("-p"))
            {
                string domain = Environment.GetEnvironmentVariable("aws_domain_name")
                    ?? throw new EnvironmentVarNotSet("The following is unset", "aws_domain_name");
                PasswordHasher.OutputNew(domain);
                return;
            }

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build();

            host.Run();
        }
    }
}