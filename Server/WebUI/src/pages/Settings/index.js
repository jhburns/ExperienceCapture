import React, { Component } from 'react';

import Menu from 'components/Menu';
import SignOutButton from 'components/SignOutButton';
import GetSignUpLink from 'components/GetSignUpLink';

import { signOutUser } from 'libs/userManagement';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'components/SingleSession/style';

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
        <Container>
          <Menu />
          <Row className="justify-content-center mb-3">
            <Col xs={7}>
              <SignOutButton onClickCallback={this.signOutCallback} />
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={7}>
              <GetSignUpLink />
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default SettingsPage;