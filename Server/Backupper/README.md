# Backupper

This is just two scripts, one for backup and one for restore. Uses MongoDB and AWS command line client in a container.

`docker-compose run bu` will backup the MongoDB instance to S3. The backup gets dumped into the same bucket for testing, staging, and production, but different folders for each.

[comment]: <> (TODO: get this working in production)
Run the restore script with `docker-compose run bu restore-mongodb-from-s3.sh [s3://path_to_restore_from_here]`