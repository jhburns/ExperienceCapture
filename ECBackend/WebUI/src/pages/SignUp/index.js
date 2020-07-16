import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper, Logo, } from 'pages/NormalSignIn/style';

import queryString from 'query-string';

import Footer from "components/Footer";

import { environmentVariables } from "libs/environment";

class SignUpPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      signUpToken: null,
    };
  }

  componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    this.setState({
      signUpToken: query.signUpToken
    });
  }

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
              <GoogleSignIn
                clientId={environmentVariables["REACT_APP_GOOGLE_CLIENT_ID"]}
                signUpToken={this.state.signUpToken}
              />
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default SignUpPage;
