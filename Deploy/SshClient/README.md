# SSH Client

This is a basic ssh client, which should only be used for debugging. The account this connects to has full access the Docker daemon on the server.

Disable SSH access by setting the environmental variable `packer_debug_option` to false in the `Deploy/.deploy.env` file.