import React, { Component } from 'react';

import { Wrapper, Info, Item } from 'components/Tutorial/style';

import { H2, A, Ol } from '@bootstrap-styled/v4';

class Tutorial extends Component {
  render() {
    return (
      <Wrapper>
        <Info className="rounded align-middle p-4">
          <H2 className="mt-0 mb-2">
            How to Start a New Session
          </H2>
          <Ol className="m-0">
            <Item>
              Follow{' '}
              <A
                href="https://github.com/jhburns/ExperienceCapture/blob/master/Documentation/Setup.md"
                target="_blank" rel="noopener noreferrer"
              >
                this tutorial
              </A>
              {' '}to install the client into your game, if not done already.
            </Item>
            <Item>Make sure that "Offline Mode" on the "SetupCapture" object is unchecked if you are in the Unity Editor.</Item>
            <Item>
              Start the game, input the URL for this website,{' '}
              <A href={window.location.origin} target="_blank" rel="noopener noreferrer">
                {window.location.origin}
              </A>
              , and click "Sign In".
            </Item>
            <Item>
              Sign in through your browser, and the new session should show up above.
            </Item>
          </Ol>
        </Info>
      </Wrapper>
    );
  }
}

export default Tutorial;