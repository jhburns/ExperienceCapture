# Partial Deploy

This tutorial assume someone has already given you all of the files and usernames/passwords to deploy. First, download a server zip from the [releases](https://github.com/jhburns/ExperienceCapture/releases).

## Create and Copy Environment Files

Create the following files and copy their provided contents into each:

- `ECBackend/.env`
- `Deploy/.env`
- `Deploy/.deploy.env`

## Change Build Arg

In the file `ECBackend/docker-compose.yaml` you may need to change Google client id if it is supplied, on the following line:

```yaml
  web:
    build:
      context: ./WebUI
      args:  # May need to change REACT_APP_GOOGLE_CLIENT_ID
        REACT_APP_GOOGLE_CLIENT_ID: [replace this part].apps.googleusercontent.com
```

## Deploying

All of this should be done in the `Deploy/` folder.

1. `docker-compose build` to build the containers.
1. `docker-compose run ssh_setup` to create an ssh key for debugging.
1. `docker-compose up packer` which builds the Amazon Machine Image (AMI).
1. Copy the image name (ex "ec-staging-v1.1.3-2020.02.01-00..27..53") into the environmental variable `aws_deploy_ami_name` in the `Deploy/.deploy.env` file to tell Pulumi which AMI to deploy.
1. `docker-compose run pulumi up` and follow the prompts to select the correct stack, which will create the needed cloud resources automatically.

## Cleanup

When done with the service, take it down with `docker-compose pulumi destroy`. Also manually delete the AMI if it no longer needs to be used by logging into the AWS Console and deleting it there.

## Changing Deploy Target

When changing deploy target to production or staging, check that the following are done:

- Change `aws_domain_name` in the `ECBackend/.env` file to reflect the domain it is being deployed too.
- Set `packer_debug_option` in the `Deploy/.deploy.env` file to false if deploying to production to follow best security practices.
- Change `aws_deploy_target` in the `ECBackend/.deploy.env` file to the correct one, either production or staging.
- Rebuild the AMI with `docker-compose up packer`.
