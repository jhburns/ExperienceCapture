#!/bin/bash

# Get the license and build first because we have too
# shellcheck disable=SC2154
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -quit  \
    -batchmode \
    -silent-crashes \
    -username "$username" \
    -password "$password" \
    -serial "$serial"

# Run a build with output
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