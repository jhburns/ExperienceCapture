import React, { Component } from 'react';

import Menu from 'components/Menu';
import GetSignUpLink from 'components/GetSignUpLink';

import { signOutUser } from 'libs/userManagement';

import {
  Container,
  Row,
  Col,
  Button,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  P,
  H2,
} from '@bootstrap-styled/v4';

import { Wrapper } from 'components/SingleSession/style';

import Footer from "components/Footer";

import { getUserId } from 'libs/userManagement';
import { getData, deleteData } from 'libs/fetchExtra';

import UserList from 'components/UserList';

import { withRouter } from "react-router";

class SettingsPage extends Component {
  constructor(props) {
    super(props);

    this.state = {
      user: null,
      isOpen: false,
      error: null,
    };

    this.onSignOut = this.onSignOut.bind(this);
    this.onDelete = this.onDelete.bind(this);
    this.toggle = this.toggle.bind(this);
  }

  async onSignOut() {
    try {
      // Whether this is mock is unknown because that state is in a different component
      signOutUser(() => {
        const { history } = this.props;
        history.push('/');
      }, (err) => this.setState({ error: err }));
    } catch (err) {
      this.setState({ error: err });
    }
  }

  async onDelete() {
    const url = `/api/v1/users/${this.state.user.id}/`;
    const request = await deleteData(url);

    if (!request.ok) {
      this.setState({ error: new Error(request.status) });
    }

    const { history } = this.props;
    history.push('/');
  }

  toggle() {
    this.setState({
      isOpen: !this.state.isOpen,
    });
  }

  async componentDidMount() {
    // eslint-disable-next-line
    const id = getUserId(async (id) => {
      const url = `/api/v1/users/${id}/`;
      const request = await getData(url);

      if (!request.ok) {
        this.setState({ error: new Error(request.status) });
        return;
      }

      const userData = await request.json();

      this.setState({
        user: userData,
      });
    }, (err) => {
      this.setState({ error: err });
    });
  }

  render() {
    if (this.state.error) {
      throw this.state.error;
    }

    let isAdmin = false;
    if (this.state.user !== null) {
      isAdmin = this.state.user.role === "Admin";
    }

    return (
      <Wrapper>
        <Menu />
        <Container className="pb-5">
          <Row className="justify-content-center">
            <Col xs={12} lg={10}>
              <Row className="mb-5 pt-3">
                <Col>
                  <H2>Settings</H2>
                </Col>
              </Row>
              <Row className="mb-5 justify-content-center">
                <Col xs={8} lg={4} className="text-center text-lg-left">
                  <Button onClick={this.onSignOut} data-cy="sign-out" size="lg">
                    Sign Out
                  </Button>
                </Col>
                <Col xs={12} className="mb-3 mb-lg-0 d-lg-none"></Col>
                <Col xs={10} lg={8}>
                  Sign out to change accounts.
                </Col>
              </Row>
              <Row className="mb-5 justify-content-center">
                <Col xs={8} lg={4} className="text-center text-lg-left">
                  <GetSignUpLink />
                </Col>
                <Col xs={12} className="mb-3 mb-lg-0 d-lg-none"></Col>
                <Col xs={10} lg={8}>
                  <P>Share the generated link with another person so they can sign up for Experience Capture.</P>
                </Col>
              </Row>
              <Row className="mb-5 justify-content-center">
                <Col xs={8} lg={4} className="text-center text-lg-left">
                  <Button onClick={this.toggle} color="warning" data-cy="delete-account" size="lg">
                    Delete Account
                  </Button>
                </Col>
                <Col xs={12} className="mb-3 mb-lg-0 d-lg-none"></Col>
                <Col xs={10} lg={8}>
                  Deleting your account will not delete any data associated with it.
                </Col>
                <Modal isOpen={this.state.isOpen} toggle={this.toggle}>
                  <ModalHeader toggle={this.toggle}>Delete Confirmation</ModalHeader>
                  <ModalBody>
                    Are you sure you want to delete your account?
                    No session data associated with this account will be lost.
                  </ModalBody>
                  <ModalFooter>
                    <Button color="warning" data-cy="confirm-delete" onClick={this.onDelete}>Delete</Button>
                    <Button color="secondary" data-cy="cancel-delete" onClick={this.toggle}>Cancel</Button>
                  </ModalFooter>
                </Modal>
              </Row>
              {isAdmin && <UserList />}
            </Col>
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

const SettingsPageWithRouter = withRouter(SettingsPage);

export default SettingsPageWithRouter;