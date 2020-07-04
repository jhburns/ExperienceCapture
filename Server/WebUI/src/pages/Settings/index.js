import React, { Component } from 'react';

import Menu from 'components/Menu';
import SignOutButton from 'components/SignOutButton';
import GetSignUpLink from 'components/GetSignUpLink';

import { signOutUser } from 'libs/userManagement';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'components/SingleSession/style';

import Footer from "components/Footer";

import { getUserId } from 'libs/userManagement';
import { getData } from 'libs/fetchExtra';

import UserList from 'components/UserList';

class SettingsPage extends Component {
  constructor(props) {
    super(props)
    
    this.state = {
      isAdmin: false,
    };

    this.onSignOut = this.onSignOut.bind(this);
  }

  async onSignOut() {
    await signOutUser(undefined); // Whether this is mock is unknown because that state is in a different component
    this.props.history.push('/');
  }

  async componentDidMount() {
    const id = getUserId();

    const url = `/api/v1/users/${id}/`;
    const request = await getData(url);
    
    if (!request.ok) {
      throw new Error(`Getting user information failed.`);
    }

    const userData = await request.json();

    this.setState({
      isAdmin: userData.role === "Admin",
    });
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
              <SignOutButton onClickCallback={this.onSignOut} />
            </Col>
          </Row>
          {this.state.isAdmin && <UserList />}
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default SettingsPage;