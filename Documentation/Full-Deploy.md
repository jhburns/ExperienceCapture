# Deploy From Scratch

## Perquisites

Doing a full deploy requires:
- Two domain names, this tutorial will cover setting up through one purchased [Namecheap](https://www.namecheap.com/), although any register can be used.
- An [AWS account](https://aws.amazon.com/#).
- A [GCP account](https://console.cloud.google.com/).
- A [Pulumi account](https://www.pulumi.com/).

Of these resources only the AWS account should cost you money, besides the domain names.
Domain names can be very cheap, `expcap.xyz` and `expcap2.xyz` in total costs $2.
A deploy can be done with only one domain name, but two is recommend so that a staging
environment can also be created.

## Downloading a Server Package

Go to the [releases page](https://github.com/jhburns/ExperienceCapture/releases) and download `Server.zip`
from one of the 'Server' releases. The most recent server and client versions should work together.
Extract the zip.

## Create Env Files

Next, the environmental files have to be created and then have their template's content copied into them.
This is the mapping:

- `Server/template.env` -> `Server/.env`
- `Deploy/template.env` -> `Deploy/.env`
- `Deploy/template.deploy.env` -> `Deploy/.deploy.env`

The rest of the tutorial will cover what each value in the environment file is for and how to
get those values from a cloud provider when applicable. Anything in square brackets, `[]` should
be replaced with your values.

## Create AMI Users

For each of the following policies in JSON:
    - Create an [AWS AMI Policy](https://docs.aws.amazon.com/IAM/latest/UserGuide/access_policies_create.html) from the following json.
    - Attach the policy to a user.
    - Copy the [Access Key ID and Secret Access Key](https://docs.aws.amazon.com/general/latest/gr/aws-sec-cred-types.html) into their respective environment variable.

The point of doing all this to follow best security practices. By making a separate policy for each service each service has only the cloud resources it needs to 
be able to operate. 

### Packer

```json
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


In `Deploy/.env` file:
```
aws_packer_access_id=[Access Key ID for Packer account]
aws_packer_secret_key=[Secret Access Key for Packer account]
aws_user_for_packer=[Username of Packer account]
```

### REX-Ray

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


In `Deploy/.env` file:
```
aws_rexray_access_id=[Access Key ID for REX-Ray account]
aws_rexray_secret_key=[Secret Access Key for REX-Ray account]
```

### Pulumi

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


In `Deploy/.env` file:
```
AWS_ACCESS_KEY_ID=[Access Key ID for Pulumi account]
AWS_SECRET_ACCESS_KEY=[Secret Access Key for Pulumi account]
```

### Backupper

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "s3:CreateBucket",
                "s3:GetObject",
                "s3:PutObject"
            ],
            "Resource": [
                "*"
            ]
        }
    ]
}
```


In `Server/.env` file:
```
aws_backupper_access_id=[Access Key ID for Backupper account]
aws_backupper_secret_key=[Secret Access Key for Backupper account]
```

## AWS Account ID

Get your [AWS Account ID](https://docs.aws.amazon.com/IAM/latest/UserGuide/console_account-alias.html) and copy it into the `aws_account_id` variable in the `Deploy/.deploy.env` file. This is done so Pulumi knows which user to search for to locate the right AMI.

## AWS Elastic IP

Allocate two [Elastic IPs](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/elastic-ip-addresses-eip.html), and fill in the following environment variables in the `Deploy/.deploy.env` file:

```
aws_production_deploy_ip_allocation_id=eipalloc-[Ip that will be mapped to production domain, ex expcap.xyz]	
aws_staging_deploy_ip_allocation_id=eipalloc-[Ip that will be mapped to staging domain, ex expcap2.xyz]
```

Allocating IPs is so that the EC2 instance can be destroyed and not need a DNS change on recreate. Now using AWS done, but keep the Elastic IP's IP address around for the next step.

## Namecheap/DNS Setup

Go to Dashboard > Manage > Advanced DNS and create a new record. It should have the following values:

| Type     | Host | Value                       | TTL       |
|----------|------|-----------------------------|-----------|
| A Record | @    | [Your IP, ex 13.52.144.154] | Automatic |

This should be the only record for the domain, and needs to be done for both domains. What this does is connect each domain to its respective domain. All traffic will be forward to these domains.

## Pulumi

Go to Profile Icon > Settings > Access Tokens > New Access Token and copy the token into the environmental variable `PULUMI_ACCESS_TOKEN` in the `Deploy/.env` file. This allows Pulumi to manage resources in the cloud, so for example two different people try to deploy at the same time.

## Google Sign-In

Go [here](https://developers.google.com/identity/sign-in/web/sign-in#before_you_begin) and select Configure a project. Then get a client id by going [to the console](https://console.developers.google.com/) then Credentials, and copying "Web client (Auto-created for Google Sign-in)". It should be copied into `Server/.env` AND `Server/docker-compose.yaml`. Copy it into this part of the compose file:

```yaml
  web:
    build:
      context: ./WebUI
      args:  # Make sure to change the following arg for different websites
        REACT_APP_GOOGLE_CLIENT_ID: [replace this part].apps.googleusercontent.com
```

Next, add both of the domains by clicking on "Web client (Auto-created for Google Sign-in)" > + ADD URI. Doing this is so the service can use normal Google account to sign users in.

## Change Domain

Set `aws_domain_name` variable in the `Server/.env` file to whichever domain is be deployed with, ex expcap.xyz. This is the domain used by Caddy to get Let's Encrypt certificated.

## Generate Password

Run `docker-compose run api dotnet API.dll --passwordGenerate` and copy the 'Hash' value into the `admin_password_hash` variable in the `Server/.env`file. This is used to bootstrap the website by visiting, `http://[your domain]/admin?password=[Password for URL value]` in the browser, two preformatted versions are print for convenience.

## Optional Environmental Variables

The following variables aren't required to be changed in order to deploy.

In `Server/.deploy.env`:
- `aws_region_name` where to deploy the service too, default us-west-1.
- `aws_ami_version_number` version to give built AMI, ex 1.1.4 .
- `packer_debug_option` whether to allow ssh access the server, either true or false.
- `aws_deploy_target` what mode to deploy in, either production or stating.
- `aws_host_ssh_address` where for the ssh client to connect to, ex expcap.xyz .

In `Deploy/.env`:
- `aws_region_name=us-west-1` same as above, default us-west-1.
- `aws_backup_bucket_name` may need to be changed to a different bucket name, because they are globally namespaced.

## Sharing and Deploying

Everything should be setup now. This setup is designed so that these files can be shared with other developers. Test that everything is working by [following a normal deploy](Partial-Deploy.md).

[comment]: <> (TODO: add info about renaming S3 bucket since those are globally namespaced)
[comment]: <> (Or fix the backupper so a prefix is used)