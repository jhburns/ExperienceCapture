import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper, Logo, } from 'pages/NormalSignIn/style';

class NormalSignInPage extends Component {
  render() {
    return (
      <Wrapper>
        <Container>
          <Row className="justify-content-center mb-4 pb-2" noGutters={true}>
            <Col>
              <Logo src={logo} alt="logo" />
            </Col>
          </Row>
          <Row noGutters={true} className="mb-5">
            <Col>
              <h1 className="text-center">
                Experience <br /> Capture 
              </h1>
            </Col>
          </Row>
          <Row className="justify-content-center" noGutters={true}>
            <Col xs={10} >
              <GoogleSignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} />
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default NormalSignInPage;
