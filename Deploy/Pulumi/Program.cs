// Code is adapted from example here: https://github.com/pulumi/examples/blob/master/aws-cs-webserver/Program.cs

// - Name: Jonathan Hirokazu Burns
// - ID: 2288851
// - email: jburns@chapman.edu
// - Course: 353-01
// - Assignment: Submission #4
// - Purpose: Manages Cloud State
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Deploy.App.CustomExceptions;

using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
using Pulumi.Aws.Inputs;

// Program class called by Pulumi
internal class Program
{
    // Not using the free tier
    private const string Ec2Size = "t2.medium";

    // Main
      // Required by Pulumi to run things
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

            var group = new SecurityGroup("experience-capture-security-group", new SecurityGroupArgs
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

            var server = new Instance("experience-capture-cloud", new InstanceArgs
            {
                InstanceType = Ec2Size,
                SecurityGroups = { group.Name },
                Ami = ami.Id,
                AssociatePublicIpAddress = false,
            });

            var elasticIp = new EipAssociation("experience-capture-ip", new EipAssociationArgs
            {
                AllocationId = ipId,
                InstanceId = server.Id,
            });

            return new Dictionary<string, object>
            {
                { "publicIp", elasticIp.PublicIp },
                { "publicDns", server.PublicDns },
            };
        });
    }
}
