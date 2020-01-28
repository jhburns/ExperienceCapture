#!/bin/sh

if [ -z "$1" ]
  then
    echo "No s3 location supplied"
    exit 1
fi

set -e
HOST=db

# shellcheck disable=SC2154
export AWS_ACCESS_KEY_ID=$aws_backupper_access_id \
    AWS_SECRET_ACCESS_KEY=$aws_backupper_secret_key \
    AWS_DEFAULT_REGION=$aws_region_name

aws2 s3 cp "$1" - \
    |  /usr/bin/mongorestore --host $HOST --archive --gzip