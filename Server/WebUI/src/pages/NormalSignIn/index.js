import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper, } from 'pages/NormalSignIn/style';

class NormalSignInPage extends Component {
  render() {
    return (
      <Wrapper>
        <Row className="justify-content-center" noGutters={true}>
          <Col xs={4} >
            <img src={logo} alt="logo" />
          </Col>
        </Row>
        <Row noGutters={true} className="mt-3">
          <Col></Col>
          <Col xs={10} >
            <h1 className="text-center">
              Experience <br /> Capture 
            </h1>
          </Col>
          <Col></Col>
        </Row>
        <Row noGutters={true}>
          <GoogleSignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} />
        </Row>
      </Wrapper>
    );
  }
}

export default NormalSignInPage;
