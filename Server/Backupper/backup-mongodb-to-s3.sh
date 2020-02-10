#!/bin/sh

# Inspired by: 
# https://gist.github.com/eladnava/96bd9771cd2e01fb4427230563991c8d#file-mongodb-s3-backup-sh-L42 
# https://gist.github.com/caraboides/7679bb73f4f13e36fc2b9dbded3c24c0

set -e
HOST=db
DB=ec
TIME=$(/bin/date --utc +%FT%TZ) # ISO8601 UTC timestamp
# shellcheck disable=SC2154
FILENAME="${TIME}.gz"

# shellcheck disable=SC2154
export AWS_ACCESS_KEY_ID=$aws_backupper_access_id \
    AWS_SECRET_ACCESS_KEY=$aws_backupper_secret_key \
    AWS_DEFAULT_REGION=$aws_region_name

# shellcheck disable=SC2154
S3PATH="s3://${aws_backup_bucket_name}/${aws_deploy_target}/${FILENAME}"

# Storage Class is Infrequently Accessed, multiple zones
# See: https://docs.aws.amazon.com/AmazonS3/latest/dev/storage-class-intro.html
/usr/bin/mongodump --archive --gzip -d $DB -h $HOST \
    | aws s3 cp --storage-class="STANDARD_IA" - "$S3PATH"