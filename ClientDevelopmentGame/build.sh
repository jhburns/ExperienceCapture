#!/bin/bash

# Get the license and build first because we have too
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -quit  \
    -batchmode \
    -silent-crashes \
    # shellcheck disable=SC2154
    -username "$USERNAME" \
    # shellcheck disable=SC2154
    -password "$PASSWORD" \
    # shellcheck disable=SC2154
    -serial "$SERIAL"

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