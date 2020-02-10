import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import { Container, Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper } from 'pages/Claim/style';

import queryString from 'query-string';

class ClaimPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      claimToken: null,
    }
  }

  componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    this.setState({
      claimToken: query.claimToken
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
                clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID}
                claimToken={this.state.claimToken}
              />
            </Col>
          </Row>
        </Container>
      </Wrapper>
    )
  }
}

export default ClaimPage;
