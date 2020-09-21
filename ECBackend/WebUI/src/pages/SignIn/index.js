import React, { Component } from 'react';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col } from '@bootstrap-styled/v4';
import { Wrapper, Illustration } from 'pages/SignIn/style';

import Footer from "components/Footer";
import Header from 'components/Header';

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
          <Header/>
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
