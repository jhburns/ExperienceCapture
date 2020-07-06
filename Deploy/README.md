# Deploy

## Setup

### Env Files

Follow the tutorial [here](https://github.com/jhburns/ExperienceCapture/blob/master/Documentation/Partial-Deploy.md#create-and-copy-environment-files).

### Initial Setup

1. Run `docker-compose build`.
1. Finally, run `docker-compose run ssh_setup` before any of the other commands. It generates SSH keys that will be used is the ssh debug key is set to true.

## Usage

These commands need to be ran in-order, more or less.

- Build an [AMI](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AMIs.html): `docker-compose up packer` which bakes a new AMI using [Packer](https://www.packer.io/) and [Ansible](https://www.ansible.com/).
- Provision resources: `docker-compose run pulumi up` and follow the prompts to deploy the application.
- Connect to Instance: `docker-compose up ssh_connect` starts a terminal session on the ECBackend. Change `aws_host_location` in *.deploy.env* to specify the IP/hostname to connected to.
- Delete resources: `docker-compose run pulumi destroy` and follow the prompts to destroy the application.

### Pulumi Tips

Any command can be passed to Pulumi, see https://www.pulumi.com/docs/reference/cli/.

## Deployment Targets

In order to change deploy targets (`staging` or `production`) follow these steps:

- Change the `aws_domain_name` environmental variable in the `ECBackend/.env` file.
- Change the `aws_deploy_target` environmental variable in the `Deploy/.deploy.env` file. It should only have the values `staging` or `production`.
- Update `aws_host_ssh_address` if you want ssh access.
- Rebuild and redeploy using the above commands.

### Staging versus Production

It is important for testing to keep the staging as close to production as possible, but they have the following differences:

- They use different [Elastic IP Addresses](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/elastic-ip-addresses-eip.html) due to having different domains.
- The `staging` version has significantly less storage space allocated for it than `production`.
- They back-up MongoDB to different folders in the S3 bucket.