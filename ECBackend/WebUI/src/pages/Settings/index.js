import React, { Component } from 'react';

import Menu from 'components/Menu';
import SignOutButton from 'components/SignOutButton';
import GetSignUpLink from 'components/GetSignUpLink';

import { signOutUser } from 'libs/userManagement';

import { Container, Row, Col, Button, } from '@bootstrap-styled/v4';

import { Wrapper } from 'components/SingleSession/style';

import Footer from "components/Footer";

import { getUserId } from 'libs/userManagement';
import { getData, deleteData } from 'libs/fetchExtra';

import UserList from 'components/UserList';

class SettingsPage extends Component {
  constructor(props) {
    super(props)
    
    this.state = {
      user: null,
    };

    this.onSignOut = this.onSignOut.bind(this);
    this.onDelete = this.onDelete.bind(this);
  }

  async onSignOut() {
    await signOutUser(undefined); // Whether this is mock is unknown because that state is in a different component
    this.props.history.push('/');
  }

  async onDelete() {
    const url = `/api/v1/users/${this.state.user.id}/`;
    const request = await deleteData(url);

    if (!request.ok) {
      throw new Error(request.status);
    }

    this.props.history.push('/');
  }

  async componentDidMount() {
    const id = getUserId();

    const url = `/api/v1/users/${id}/`;
    const request = await getData(url);
    
    if (!request.ok) {
      throw new Error(request.status);
    }

    const userData = await request.json();

    this.setState({
      user: userData,
    });
  }

  render() {
    let isAdmin = false;
    if (this.state.user !== null) {
      isAdmin = this.state.user.role === "Admin";
    }

    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="m-0 justify-content-center mb-3">
            <Col xs={7} md={4} xl={3}>
              <GetSignUpLink />
            </Col>
          </Row>
          <Row className="m-0 justify-content-center mb-3">
            <Col xs={7} md={4} xl={3}>
              <SignOutButton onClickCallback={this.onSignOut} />
            </Col>
          </Row>
          {/* TODO: add pop-up asking for confirmation */}
          <Row className="m-0 justify-content-center">
            <Col xs={7} md={4} xl={3}>
              <Button
                className="btn btn-danger btn-block"
                onClick={this.onDelete}
                data-cy="delete-account"
              >Delete Account</Button>
            </Col>
          </Row>
          {isAdmin && <UserList />}
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default SettingsPage;