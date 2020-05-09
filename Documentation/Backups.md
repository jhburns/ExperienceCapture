# Backups

## Restore

The following steps should be followed to ensure a safe restore:

1. Backup the current MongoDB state if possible, either by waiting a day for an automatic backup or running the Backupper command in production. Taking a snapshot of the EBS volume is also a good idea.
1. Mirror MongoDB by deleting the local database volume, and running the restore script pointed at a production dump. Example commands, executed in the `Server/` folder:

    ```shell
    $ docker-compose down
    $ docker volume rm server_ec-db-volume
    $ docker-compose run bu ./restore-mongodb-from-s3.sh s3://experiencecapture-ec-db-backups/production 2020-02-10T20:21:41Z.gz
    ```

1. Run the following commands to reset every session to be set as not exported. This has to be done because Minio isn't backed up so it may result in inconsistent state and it is easier to re-export as needed.

    ```shell
    $ docker-compose up rp
    $ docker exec -it server_db_1 mongo
    > use ec
    > db.sessions.updateMany({}, { $set: { "isExported" : false, "isPending" false } })
    ```

1. Dump local MongoDB into S3 using the Backupper, with the command `docker-compose run bu`.
1. Test that the database can be restored from staging successfully. Use the following command to restore in staging or production:

    ```shell
    $ cd /srv
    $ docker run --network=ec_ec-network --env-file=".env" 127.0.0.1:5000/ec-bu ./restore-mongodb-from-s3.sh s3://experiencecapture-ec-db-backups/development/[Backup filename here].gz
    ```

1. Delete both MongoDB and Minio's production EBS volumes, then restore like in staging.