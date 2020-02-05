# Backupper

This is just two scripts, one for backup and one for restore. Uses MongoDB and AWS command line client in a container.

Run the restore script with `docker-compose run bun restore-mongodb-from-s3.sh [s3://path_to_restore_from_here]`