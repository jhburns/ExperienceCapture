import React, { Component } from 'react';
import logo from 'img/logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, H1 } from '@bootstrap-styled/v4';
import { Wrapper, Logo } from 'pages/SignIn/style';

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
        <Container noGutters={true} className="p-0">
          <Row className="mt-5">
            <Col className="d-flex align-items" xs="5" lg="2">
              <Logo src={logo} alt="logo" />
            </Col>
            <Col className="d-flex align-items-center">
              <H1 className="mb-0">
                Experience Capture
              </H1>
            </Col>
          </Row>
          {/*
          <Row className="justify-content-center" noGutters={true}>
            <Col xs={10} >
              <GoogleSignIn
                claimToken={this.state.claimToken}
                signUpToken={this.state.signUpToken}
              />
            </Col>
          </Row>
          <Row>
            <Col>
              <img src={allTheData} alt="All of the data." />
            </Col>
          </Row>
          <Footer />
          */}
        </Container>
      </Wrapper>
    );
  }
}

export default SignInPage;
