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
echo Run build
echo ----------------------------------------------------------
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -quit  \
    -batchmode \
    -projectPath \
    -logFile - \
    -buildTarget Win64 \
    /app

# Run tests
echo Run tests
echo ----------------------------------------------------------
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
	-runTests \
    -batchmode \
    -projectPath \
	-testResults ./unit-tests.xml

cat unit-tests.xml

status2=$?

# Return license
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity -quit -batchmode -returnlicense -silent-crashes

exit "$status2"