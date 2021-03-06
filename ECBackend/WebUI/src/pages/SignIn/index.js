import React, { Component } from 'react';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, P } from '@bootstrap-styled/v4';
import { Wrapper, Illustration, PromoTitle } from 'pages/SignIn/style';

import Footer from "components/Footer";
import Header from 'components/Header';

import queryString from 'query-string';

import allTheData from 'img/all_the_data.svg';

class SignInPage extends Component {
  render() {
    return (
      <Wrapper>
        <Container className="pb-3">
          <Header/>
          <Row noGutters>
            <Col xs={12} lg={4}>
              <PromoTitle className="mb-3 d-none d-lg-block mt-2">
                Data collection you deserve
              </PromoTitle>
              <Row>
                <Col xs={12} className="d-none d-lg-block mb-5">
                  <P>
                    Experience Capture is a research oriented data analytics platform for Unity games.
                    Capture any data you want, and analyze it using whatever tools you prefer.
                  </P>
                </Col>
                <Col lg={12} className="mb-5 mb-lg-0 d-lg-none text-center">
                  <Illustration src={allTheData} alt="All of the data." />
                </Col>
                <Col xs={12} className="justify-content-center mb-5">
                  <GoogleSignIn
                    signUpToken={queryString.parse(this.props.location.search).signUpToken}
                  />
                </Col>
              </Row>
            </Col>
            <Col lg={8} className="mb-5 mb-lg-0 d-none d-lg-block text-center">
              <Illustration src={allTheData} alt="All of the data." />
            </Col>
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default SignInPage;
