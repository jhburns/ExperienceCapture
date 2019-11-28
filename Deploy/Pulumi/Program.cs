// Code is adapted from example here: https://github.com/pulumi/examples/blob/master/aws-cs-webserver/Program.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Deploy.App.CustomExceptions;

using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
using Pulumi.Aws.Inputs;

internal class Program
{
    // Not using the free tier
    private const string Ec2Size = "t2.medium";

    private static Task<int> Main()
    {
        return Deployment.RunAsync(async () =>
        {
            string awsId = Environment.GetEnvironmentVariable("aws_account_id")
                ?? throw new EnviromentVarNotSet("The following is unset", "aws_account_id");

            string amiName = Environment.GetEnvironmentVariable("aws_deploy_ami_name")
                ?? throw new EnviromentVarNotSet("The following is unset", "aws_deploy_ami_name");

            string ipId = Environment.GetEnvironmentVariable("aws_deploy_ip_allocation_id")
                ?? throw new EnviromentVarNotSet("The following is unset", "aws_deploy_ip_allocation_id");

            var ami = await Pulumi.Aws.Invokes.GetAmi(new GetAmiArgs
            {
                MostRecent = true,
                Owners = { awsId },
                Filters =
                {
                    new GetAmiFiltersArgs
                    {
                        Name = "name",
                        Values = { amiName },
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
                new SecurityGroupIngressArgs
                {
                    Protocol = "tcp",
                    FromPort = 22,
                    ToPort = 22,
                    CidrBlocks = { "0.0.0.0/0" },
                },
                new SecurityGroupIngressArgs
                {
                    Protocol = "tcp",
                    FromPort = 443,
                    ToPort = 443,
                    CidrBlocks = { "0.0.0.0/0" },
                },
            },
                Egress =
            {
                new SecurityGroupEgressArgs
                {
                    Protocol = "-1",
                    FromPort = 0,
                    ToPort = 0,
                    CidrBlocks = { "0.0.0.0/0" },
                },
            },
            });

            var userData = @"
#!/bin/bash
echo ""Hello, World!"" > index.html
";

            var server = new Instance("experience-capture-cloud", new InstanceArgs
            {
                InstanceType = Ec2Size,
                SecurityGroups = { group.Name },
                UserData = userData,
                Ami = ami.Id,
                AssociatePublicIpAddress = false,
            });

            var elasticIP = new EipAssociation("experience-capture-ip",new EipAssociationArgs
            {
                AllocationId = ipId,
                InstanceId = server.Id,
            });

            return new Dictionary<string, object>
            {
                { "publicElasticIp",  elasticIP.PublicIp },
                { "publicDns",  server.PublicDns },
            };
        });
    }
}
