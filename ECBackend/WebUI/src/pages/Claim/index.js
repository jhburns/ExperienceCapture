import React, { Component } from 'react';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, H2, P, A } from '@bootstrap-styled/v4';
import { Wrapper, Image } from 'pages/Claim/style';

import Footer from "components/Footer";
import Header from 'components/Header';

import queryString from 'query-string';

import checkMark from 'img/check_sign_in.svg';

import { Link } from 'react-router-dom';

class ClaimPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      signUpToken: null,
      claimToken: null,
      isClaimRedeemed: false,
    };
  }

  componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    this.setState({
      signUpToken: undefined,
      claimToken: query.claimToken,
    });
  }

  render() {
    return (
      <Wrapper>
        <Container className="pb-5">
          <Header />
          {this.state.isClaimRedeemed ?
            <Row className="justify-content-center">
              <Col xs={10} lg={4} className="mb-5">
                <GoogleSignIn
                  claimToken={this.state.claimToken}
                  onSuccessfulClaim={() => this.setState({ isClaimRedeemed: true })}
                />
              </Col>
            </Row>
            :
            <>
              <Row className="mb-5 justify-content-center">
                <Col xs="auto">
                  <Image src={checkMark} alt="Check mark."></Image>
                </Col>
              </Row>
              <Row className="mb-3 justify-content-center">
                <Col xs={12} lg="auto">
                  <H2 className="text-center">External Sign In Successful</H2>
                </Col>
              </Row>
              <Row className="justify-content-center pb-5">
                <Col xs={8} lg={4}>
                  <P>
                    Close this tab and return to your game, or go to the&nbsp;
                    <A as={Link} to="/home/start">
                        home page.
                    </A>
                  </P>
                </Col>
              </Row>
            </>
          }
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default ClaimPage;
