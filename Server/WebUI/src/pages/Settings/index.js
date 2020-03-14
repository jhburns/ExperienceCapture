import React, { Component } from 'react';

import Menu from 'components/Menu';
import SignOutButton from 'components/SignOutButton';
import GetSignUpLink from 'components/GetSignUpLink';

import { signOutUser } from 'libs/userManagement';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'components/SingleSession/style';

import Footer from "components/Footer";

class SettingsPage extends Component {
  constructor(props) {
    super(props)
    
    this.signOutCallback = this.onSignOut.bind(this);
  }

  async onSignOut() {
    await signOutUser(undefined); // Whether this is mock is unknown because that state is in a different component
    this.props.history.push('/');
  }

  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="m-0 justify-content-center mb-3">
            <Col xs={7} md={4} xl={3}>
              <GetSignUpLink />
            </Col>
          </Row>
          <Row className="m-0 justify-content-center">
            <Col xs={7} md={4} xl={3}>
              <SignOutButton onClickCallback={this.signOutCallback} />
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default SettingsPage;