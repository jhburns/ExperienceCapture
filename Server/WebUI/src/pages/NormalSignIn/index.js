import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper } from 'pages/NormalSignIn/style';

class NormalSignInPage extends Component {
  render() {
    return (
      <Wrapper>
        <Container>
          <Row className="justify-content-center" noGutters={true}>
            <Col xs={4} >
              <img src={logo} alt="logo" />
            </Col>
          </Row>
          <Row noGutters={true} className="mt-3 mb-5">
            <Col></Col>
            <Col xs={10} >
              <h1 className="text-center">
                Experience <br /> Capture 
              </h1>
            </Col>
            <Col></Col>
          </Row>
          <Row className="justify-content-center" noGutters={true}>
            <Col xs={8} >
              <GoogleSignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} />
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default NormalSignInPage;
