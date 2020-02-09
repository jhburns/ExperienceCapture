# Deploy

## Setup

### Env Files

Follow the tutorial [here](https://github.com/jhburns/ExperienceCapture/blob/master/Documentation/Partial-Deploy.md#create-and-copy-environment-files).

### Build

1. Run `docker-compose build`. 
1. Finally, run `docker-compose run ssh_setup` before any of the other commands. It generates SSH keys that will be used is the ssh debug key is set to true.

## Usage

- Build an [AMI](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AMIs.html): `docker-compose up packer` builds a new AMI using [Packer](https://www.packer.io/) and [Ansible](https://www.ansible.com/).
- Provision resources: `docker-compose run pulumi up` and follow the prompts to deploy the application.
- Connect to Instance: `docker-compose up ssh_connect` starts a terminal session to the Server. Change `aws_host_location` in *info.env* to specify the IP/hostname this is to be connected to.
- Delete resources: `docker-compose run pulumi destroy` and follow the prompts to destroy the application.

### Pulumi Tip

Any command can be passed to Pulumi, see https://www.pulumi.com/docs/reference/cli/.

## Security Concerns

This deployment setup generates an SSH key pair locally through the ssh_setup, which is
placed in *./.secrets*. Keep in mind that sharing/allowing access to the keys is **dangerous**
as those keys are baked into the AMI. Although the account using them in the running instance only
really has access to Docker, keep in mind it is still very privileged access. 

Revoking a key can be done by re-running `docker-compose run ssh_setup`, selecting 'y' and then
rebaking/deploying the full application. 

Additionally, do not share/publish any of the variables found in the *.env* file because they 
allow access to various cloud services. 

In terms of the api tokens/keys being leaked the service should be taken down, all of the tokens revoked and reissued.
Then rebake/deploy the full application.