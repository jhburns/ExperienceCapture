// Code is adapted from example here: https://github.com/pulumi/examples/blob/master/aws-cs-webserver/Program.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
using Pulumi.Aws.Inputs;

internal class Program
{
    // Not using the free tier
    private const string Ec2Size = "t3.medium";

    private static Task<int> Main()
    {
        return Deployment.RunAsync(async () =>
        {
            string awsId = Environment.GetEnvironmentVariable("aws_account_id") ?? "(not specified)";
            string amiName = Environment.GetEnvironmentVariable("aws_deploy_ami_name") ?? "(not specified)";

            var ami = await Pulumi.Aws.Invokes.GetAmi(new GetAmiArgs
            {
                MostRecent = true,
                Owners = { awsId },
                Filters =
                {
                    new GetAmiFiltersArgs
                    {
                        Name = amiName,
                        Values = { "amzn-ami-hvm-*" },
                    },
                },
            });

            var group = new SecurityGroup("web-secgrp", new SecurityGroupArgs
            {
                Description = "Enable HTTP access",
                Ingress =
            {
                new SecurityGroupIngressArgs
                {
                    Protocol = "tcp",
                    FromPort = 80,
                    ToPort = 80,
                    CidrBlocks = { "0.0.0.0/0" },
                },
            },
            });

            var userData = @"
#!/bin/bash
echo ""Hello, World!"" > index.html
nohup python -m SimpleHTTPServer 80 &
";

            var server = new Instance("web-server-www", new InstanceArgs
            {
                InstanceType = Ec2Size,
                SecurityGroups = { group.Name },
                UserData = userData,
                Ami = ami.Id,
            });

            return new Dictionary<string, object>
            {
                { "publicIp",  server.PublicIp },
                { "publicDns",  server.PublicDns },
            };
        });
    }
}
