import React, { Component } from 'react';
import { Link } from "react-router-dom";

import { Wrapper, Message } from 'components/ClaimNotify/style';

import { Row, Col, } from '@bootstrap-styled/v4';

class ClaimNotify extends Component {
  render() {
    return (
      <Wrapper>
        <Row className="justify-content-center">
          <Col className="text-center">
            <h3>
              <span role="img" aria-label="confirmation">✔️</span> You've authenticated the session
            </h3>
            <Message>Return to the game, or <Link to="/" data-cy="go-root">sign in.</Link></Message>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default ClaimNotify;