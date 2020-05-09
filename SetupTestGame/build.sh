#!/bin/bash

# Get the license and build first because we have too
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -quit  \
    -batchmode \
    -buildTarget Win64 \
    -silent-crashes
    -username "$username" \
    -password "$password" \
    -serial "$serial" \
    /app

status1=$?

if [ "$status1" -ne 0 ];
then
    echo "Error: Could not aquire license"
    exit "$status1"
fi

# Run a build with outut
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -quit  \
    -batchmode \
    -projectPath \
    -logFile - \
    -buildTarget Win64 \
    /app

status2=$?

# Return license
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity -quit -batchmode -returnlicense -silent-crashes

exit "$status2"