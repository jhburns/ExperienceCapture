import React, { Component } from 'react';
import logo from 'img/logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, H1 } from '@bootstrap-styled/v4';
import { Wrapper, Logo, Illustration } from 'pages/SignIn/style';

import Footer from "components/Footer";

import queryString from 'query-string';

import allTheData from 'img/all_the_data.svg';

class SignInPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      signUpToken: null,
      claimToken: null,
    };
  }

  componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    this.setState({
      signUpToken: query.signUpToken,
      claimToken: query.claimToken,
    });
  }

  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Row className="mt-5 mb-5 pb-5" noGutters={true}>
            <Col className="d-flex align-items-center pr-0" xs="5" sm="4" lg="2">
              <Logo src={logo} alt="logo"/>
            </Col>
            <Col className="d-flex align-items-center pl-0">
              <H1 className="mb-0 font-weight-bold">
                Experience Capture
              </H1>
            </Col>
          </Row>
          <Row className="mb-5 pb-3" noGutters={true}>
            <Col>
              <Illustration className="pl-2" src={allTheData} alt="All of the data." />
            </Col>
          </Row>
          <Row className="justify-content-center pb-4" noGutters={true}>
            <Col xs={10} >
              <GoogleSignIn
                claimToken={this.state.claimToken}
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

export default SignInPage;
