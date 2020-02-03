import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper } from 'pages/SignUp/style';

import queryString from 'query-string';

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
            <Col xs={10} >
              <GoogleSignIn
                clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID}
                signUpToken={this.state.signUpToken}
              />
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default SignUpPage;
