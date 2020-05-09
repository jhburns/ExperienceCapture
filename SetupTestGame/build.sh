#!/bin/bash

# Get the license and build first because we have too
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -quit  \
    -batchmode \
    -silent-crashes \
    -buildTarget Win64 \
    -username "$username" \
    -password "$password" \
    -serial "$serial" \
    /app/SetupTestGame

# Run a build with outut
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -quit  \
    -batchmode \
    -projectPath \
    -logFile - \
    -buildTarget Win64 \
    /app/SetupTestGame

status=$?

# Return license
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity -quit -batchmode -returnlicense -silent-crashes

exit "$status"