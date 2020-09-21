import React, { Component } from 'react';

import { Wrapper, Info } from 'components/LoginBox/style';

import { Row, Col, H5 } from '@bootstrap-styled/v4';

class LoginBox extends Component {
  render() {
    return (
      <Wrapper>
        <Row className="justify-content-center">
          <Col xs={10} className="mb-4">
            <Info className="align-middle p-3">
              <H5 className="text-center mb-0">
                {this.props.children}
              </H5>
            </Info>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default LoginBox;