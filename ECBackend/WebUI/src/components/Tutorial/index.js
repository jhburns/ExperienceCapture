import React, { Component } from 'react';

import { Wrapper, Info, } from 'components/Tutorial/style';

class Tutorial extends Component {
  render() {
    return (
      <Wrapper>
        <Info className="rounded align-middle">
          <h5 className="mt-0 mb-2">
            How to Start a New Session
          </h5>
          <ol>
            <li>
              Follow{' '}
              <a
                href="https://github.com/jhburns/ExperienceCapture/blob/master/Documentation/Setup.md"
                target="_blank" rel="noopener noreferrer"
              >
                this tutorial
              </a>
              {' '}to install the client into your game, if not done already.
            </li>
            <li>Uncheck "Offline Mode" on the "SetupCapture" object.</li>
            <li>
              Input the URL for this website,{' '}
              <a href={window.location.origin} target="_blank" rel="noopener noreferrer">
                {window.location.origin}
              </a>
              , and click "New Session".
            </li>
          </ol>
        </Info>
      </Wrapper>
    );
  }
}

export default Tutorial;