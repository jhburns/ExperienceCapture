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
    private const string Ec2Size = "t2.medium";

    private static readonly string AwsId = Environment.GetEnvironmentVariable("aws_account_id")
        ?? throw new EnviromentVarNotSet("The following is unset", "aws_account_id");

    private static readonly string AmiName = Environment.GetEnvironmentVariable("aws_deploy_ami_name")
        ?? throw new EnviromentVarNotSet("The following is unset", "aws_deploy_ami_name");

    private static readonly string IpId = Environment.GetEnvironmentVariable("aws_deploy_ip_allocation_id")
        ?? throw new EnviromentVarNotSet("The following is unset", "aws_deploy_ip_allocation_id");

    private static Task<int> Main()
    {
        return Deployment.RunAsync(async () =>
        {
            var ami = await Pulumi.Aws.Invokes.GetAmi(new GetAmiArgs
            {
                MostRecent = true,
                Owners = { AwsId },
                Filters =
                {
                    new GetAmiFiltersArgs
                    {
                        Name = "name",
                        Values = { AmiName },
                    },
                },
            });

            var group = new SecurityGroup("experience-capture-security-group", new SecurityGroupArgs
            {
                Description = "Enable HTTP access",
                Ingress =
            {
                CreateIngressRule(22), // SSH
                CreateIngressRule(80), // HTTP
                CreateIngressRule(443), // HTTPS
            },
                Egress =
            {
                new SecurityGroupEgressArgs // Allow all outbound traffic
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
                AllocationId = IpId,
                InstanceId = server.Id,
            });

            return new Dictionary<string, object>
            {
                { "publicIp", elasticIp.PublicIp },
                { "publicDns", server.PublicDns },
            };
        });
    }

    private static SecurityGroupIngressArgs CreateIngressRule(int port)
    {
        return new SecurityGroupIngressArgs
        {
            Protocol = "tcp",
            FromPort = port,
            ToPort = port,
            CidrBlocks = { "0.0.0.0/0" },
        };
    }
}
