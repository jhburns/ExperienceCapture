// Code is adapted from example here: https://github.com/pulumi/examples/tree/master/aws-cs-webserver#web-server-using-amazon-ec2

using System.Threading.Tasks;

using Pulumi;
using Pulumi.App.WebServerStack;

public class Program
{
    public static Task<int> Main() => Deployment.RunAsync<WebServerStack>();
}
