import React, { Component } from 'react';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col } from '@bootstrap-styled/v4';
import { Wrapper } from 'pages/Claim/style';

import Footer from "components/Footer";
import Header from 'components/Header';

import queryString from 'query-string';

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
        <Container className="p-0">
          <Header />
          <Row className="justify-content-center">
            {this.state.isClaimRedeemed ?
              <Col xs={10} lg={4} className="mb-5">
                <GoogleSignIn
                  claimToken={this.state.claimToken}
                  onSuccessfulClaim={() => this.setState({ isClaimRedeemed: true })}
                />
              </Col>
              :
              <Col></Col>
            }
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default ClaimPage;
