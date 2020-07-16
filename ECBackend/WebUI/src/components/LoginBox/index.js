import React, { Component } from 'react';

import { Wrapper, Info, } from 'components/LoginBox/style';

import { Row, Col, } from '@bootstrap-styled/v4';

class LoginBox extends Component {
  render() {
    return (
      <Wrapper>
        <Row className="justify-content-center">
          <Col xs={10} sm={8} md={6} lg={4} className="mb-4">
            <Info className="rounded align-middle">
              <h5 className="mt-0 mb-0">
                {this.props.children}
              </h5>
            </Info>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default LoginBox;