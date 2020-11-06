import React, { useRef, useState, useCallback } from 'react';

import { Wrapper } from 'components/Camera/style';

import Webcam from 'react-webcam';

import { Button } from '@bootstrap-styled/v4';

// This component has to use React hooks since the library requires it
// Unfortunately errors aren't reported right now

const Camera = () => {
  const mediaRecorderRef = useRef(null);
  const [recordedChunks, setRecordedChunks] = useState([]);
  const [error, setError] = useState(null);

  const handleDataAvailable = useCallback(
    ({ data }) => {
      if (data.size > 0) {
        setRecordedChunks((prev) => prev.concat(data));
      }
    },
    [setRecordedChunks],
  );

  const setWebcamRef = useCallback(async (node) => {
    if (node === null) {
      setError(new Error("Webcam is not found."));
      return;
    }

    // Very hacky, wait until the stream is ready
    const timeout = (ms) => new Promise(resolve => setTimeout(resolve, ms));
    while (node.stream === undefined) {
      await timeout(150);
    }

    mediaRecorderRef.current = new MediaRecorder(node.stream, {
      mimeType: "video/webm",
    });

    mediaRecorderRef.current.addEventListener(
      "dataavailable",
      handleDataAvailable,
    );

    mediaRecorderRef.current.start();
  }, [handleDataAvailable, mediaRecorderRef, setError]);

  const handleStopCaptureClick = useCallback(() => {
    mediaRecorderRef.current.stop();

    const blob = new Blob(recordedChunks, {
      type: "video/webm",
    });

    setRecordedChunks([]);
  }, [mediaRecorderRef, recordedChunks]);

  if (error !== null) {
    throw error;
  }

  return (
    <Wrapper>
      <Webcam audio={false} ref={setWebcamRef} />
      <Button onClick={handleStopCaptureClick}>Stop Capture</Button>
    </Wrapper>
  );
};

export default Camera;