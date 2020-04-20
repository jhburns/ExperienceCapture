# Backupper

This is just two scripts, one for backup and one for restore. Uses MongoDB and AWS command line client in a container.

`docker-compose run bu` will backup the MongoDB instance to S3. The backup gets dumped into the same bucket for development, staging, and production, but different folders for each.

## Policy

The current backup policy is as follows:
- Backup at 2 AM Pacific Standard Time everyday, to avoid impacting service
- Delete backups after about a month (30 days) to reduce costs.
- The latest backup shouldn't expire.

## Restore

[comment]: <> (TODO: get this working/have examples in production)
Run the restore script with:
```bash
docker-compose run bu ./restore-mongodb-from-s3.sh s3://[path to dump here]
```
For example:
```bash
docker-compose run bu ./restore-mongodb-from-s3.sh s3://experiencecapture-ec-db-backups/development/2020-02-10T20:21:41Z.gz
```