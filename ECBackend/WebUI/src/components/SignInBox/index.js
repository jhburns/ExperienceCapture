import React, { Component } from 'react';

import { Wrapper } from 'components/SignInBox/style';

import { Row, Col } from '@bootstrap-styled/v4';

import NotifyBox from 'components/NotifyBox';

class SignInBox extends Component {
  render() {
    return (
      <Wrapper>
        <Row className="justify-content-center">
          <Col xs={10} className="mb-4">
            <NotifyBox>{this.props.children}</NotifyBox>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default SignInBox;
