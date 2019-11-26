# Deploy

## Setup

1. Install [Docker](https://docs.docker.com/v17.09/engine/installation/) along with [Docker Compose](https://docs.docker.com/compose/install/).
1. Run `docker-compose build`. 
1. Copy the contents of *template.env* to a new file named *.env* then fill out its content with values from [AWS](https://docs.aws.amazon.com/general/latest/gr/aws-sec-cred-types.html#access-keys-and-secret-access-keys).
1. Finally, run `docker-compose run ssh_setup` before any of the other commands.
It generates ssh keys that will be used for the rest of the process.

### Optionally 

Change the variables in *info.env*, for example `aws_region_name` can be set to a different [AWS region](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/Concepts.RegionsAndAvailabilityZones.html).

## Usage

- Build [AMI](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AMIs.html): `docker-compose up packer` builds a new AMI using [Packer](https://www.packer.io/ and [Ansible](https://www.ansible.com/).
- Connect to Instance: `docker-compose up ssh_connect` starts a terminal session to the Server. Change `aws_host_location` in *info.env* to specify the IP/Hostname this is to be connected to.

## Security Concerns

This deployment setup generates an SSH key pair locally through the ssh_setup, which is
placed in *./.secrets*. Keep in mind that sharing/allowing access to the keys is **dangerous**
as those keys are baked into the AMI. Although the account using them in the running instance only
really has access to Docker, keep in mind it is still very privileged access. 

Revoking a key can be done by re-running `docker-compose run ssh_setup`, selecting 'y' and then
rebaking/deploying the full application. 