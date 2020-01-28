#!/bin/sh

# From: https://gist.github.com/eladnava/96bd9771cd2e01fb4427230563991c8d#file-mongodb-s3-backup-sh-L42 kinda

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

/bin/mkdir -p ./backup/archives

/usr/bin/mongodump --archive --gzip -d $DB -h $HOST | /usr/local/bin/aws2 s3 cp - "$S3PATH"