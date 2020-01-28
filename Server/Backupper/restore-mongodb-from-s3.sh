#!/bin/sh

set -e
HOST=db

export AWS_ACCESS_KEY_ID=$aws_backupper_access_id \
    AWS_SECRET_ACCESS_KEY=$aws_backupper_secret_key \
    AWS_DEFAULT_REGION=$aws_region_name

aws2 s3 cp s3://experiencecapture-ec-db-backups/development/2020-01-28T01:32:12Z.gz - \
    |  /usr/bin/mongorestore --host $HOST --archive --gzip