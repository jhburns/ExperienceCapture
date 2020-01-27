# Cloud Deploy

Hopefully this is enough steps to successfully deploy the service. 

**Important**: This tutorial requires you own and have control over a Top Level Domain.

## Requirements

- A Top Level Domain.
- An Amazon Web Services account.
- A Google Account.
- A Pulumi Account. https://www.pulumi.com/
- Docker installed on this computer.

## Create Roles

Create each users in AWS with the following roles.

Pulumi's role needs the following actions:

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "ec2:AllocateAddress",
                "ec2:AssociateAddress",
                "ec2:AttachVolume",
                "ec2:AuthorizeSecurityGroupEgress",
                "ec2:AuthorizeSecurityGroupIngress",
                "ec2:CreateSecurityGroup",
                "ec2:CreateVolume",
                "ec2:DeleteSecurityGroup",
                "ec2:DescribeAddresses",
                "ec2:DescribeAvailabilityZones",
                "ec2:DescribeImages",
                "ec2:DescribeInstanceAttribute",
                "ec2:DescribeInstanceCreditSpecifications",
                "ec2:DescribeInstanceStatus",
                "ec2:DescribeInstances",
                "ec2:DescribeNetworkInterfaces",
                "ec2:DescribeSecurityGroups",
                "ec2:DescribeSnapshots",
                "ec2:DescribeSubnets",
                "ec2:DescribeTags",
                "ec2:DescribeVolumes",
                "ec2:DescribeVpcs",
                "ec2:DetachVolume",
                "ec2:DisassociateAddress",
                "ec2:RevokeSecurityGroupEgress",
                "ec2:RevokeSecurityGroupIngress",
                "ec2:RunInstances",
                "ec2:StopInstances",
                "ec2:TerminateInstances"
            ],
            "Resource": "*"
        }
    ]
}
```

REX-Ray's:
```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "ec2:AttachVolume",
                "ec2:CopySnapshot",
                "ec2:CreateSnapshot",
                "ec2:CreateTags",
                "ec2:CreateVolume",
                "ec2:DeleteSnapshot",
                "ec2:DeleteVolume",
                "ec2:DescribeAvailabilityZones",
                "ec2:DescribeInstances",
                "ec2:DescribeSnapshotAttribute",
                "ec2:DescribeSnapshots",
                "ec2:DescribeTags",
                "ec2:DescribeVolumeAttribute",
                "ec2:DescribeVolumeStatus",
                "ec2:DescribeVolumes",
                "ec2:DetachVolume",
                "ec2:ModifySnapshotAttribute",
                "ec2:ModifyVolumeAttribute"
            ],
            "Resource": "*"
        }
    ]
}
```

Packer's
```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "ec2:AttachVolume",
                "ec2:AuthorizeSecurityGroupIngress",
                "ec2:CopyImage",
                "ec2:CreateImage",
                "ec2:CreateKeypair",
                "ec2:CreateSecurityGroup",
                "ec2:CreateSnapshot",
                "ec2:CreateTags",
                "ec2:CreateVolume",
                "ec2:DeleteKeyPair",
                "ec2:DeleteSecurityGroup",
                "ec2:DeleteSnapshot",
                "ec2:DeleteVolume",
                "ec2:DeregisterImage",
                "ec2:DescribeImageAttribute",
                "ec2:DescribeImages",
                "ec2:DescribeInstances",
                "ec2:DescribeInstanceStatus",
                "ec2:DescribeRegions",
                "ec2:DescribeSecurityGroups",
                "ec2:DescribeSnapshots",
                "ec2:DescribeSubnets",
                "ec2:DescribeTags",
                "ec2:DescribeVolumes",
                "ec2:DetachVolume",
                "ec2:GetPasswordData",
                "ec2:ModifyImageAttribute",
                "ec2:ModifyInstanceAttribute",
                "ec2:ModifySnapshotAttribute",
                "ec2:RegisterImage",
                "ec2:RunInstances",
                "ec2:StopInstances",
                "ec2:TerminateInstances"
            ],
            "Resource": "*"
        }
    ]
}
```

Backupper's
```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "s3:CreateBucket",
                "s3:PutObject"
            ],
            "Resource": ["*"]
        }
    ]
}
```

## More AWS Setup

- Make sure there is a default VPC in the region you are targeting.
- Create a new AWS Elastic IP and point your domain at it. See https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/elastic-ip-addresses-eip.html

## Download Server Files

Get the most recent release from here: https://github.com/jhburns/ExperienceCapture/releases/tag/server.1.1.3

Then unzip the folder. 

## Build

In the *Deploy/* folder run	`docker-compose build`

## Copy the ENV files

ENV files can be found in both *Deploy/* and *Server/* folders.

For each of the *.env files, copy the template into a not file with the following mappings in their same directory:
- template.host.info.env -> host.info.env
- template.credentials -> credentials
- template.env -> .env (Yes, its starts with a period)
- template.info.env -> info.env

## Fill in the Environment Info

In no particular order:
- `aws_domain_name` you domain name.
- `caddypath` keep the same.
- `REACT_APP_GOOGLE_CLIENT_ID` your Google client id. See: https://developers.google.com/identity/sign-in/web/sign-in
- `aws_region_name=us` AWS region.
- `aws_ami_name_prefix` keep the same.
- `aws_ami_version_number` keep the same.
- `aws_host_ssh_address` recommenced to be the same as `aws_domain_name` 
- `aws_account_id` your account id for AWS.
- `aws_deploy_ami_name` Change this after building the AMI.
- `aws_deploy_ip_allocation_id` Elastic IP id.
- `aws_packer_access_id` Packer account's access key. See https://docs.aws.amazon.com/IAM/latest/UserGuide/id_credentials_access-keys.html#Using_CreateAccessKey.
- `aws_packer_secret_key` Packer account's secret key. See https://aws.amazon.com/blogs/security/wheres-my-secret-access-key/
- `aws_user_for_packer` Packer account's name.
- `aws_rexray_access_id` REX-ray account's access key.
- `aws_rexray_secret_key` REX-ray account's secret key.
- `PULUMI_ACCESS_TOKEN` Get from Pulumi, see https://www.pulumi.com/docs/intro/console/accounts-and-organizations/accounts/#access-tokens
- `aws_access_key_id` Pulumi account's access key.
- `aws_secret_access_key` Pulumi account's secret key.

## Using

Run `docker-compose run ssh_setup` to generate the local SSH keys. 

Run `docker-compose up packer` in *Deploy/* to build the AMI. Then change the `aws_deploy_ami_name` ENV var to the
one returned by it.

Next, run `docker-compose run pulumi_up` and follow the prompts to create the cloud service.

Check it is running at `yourdomain.com/api/v1/`.

Finally, run `docker-compose run pulumi_destroy` and follow the prompts to take down the services. 