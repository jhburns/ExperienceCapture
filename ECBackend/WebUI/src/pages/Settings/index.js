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
        <Container nogutters className="pb-5">
          <Row className="m-0 justify-content-center mb-3">
            <Col xs={7} md={4} xl={3}>
              <GetSignUpLink />
            </Col>
          </Row>
          <Row className="m-0 justify-content-center mb-3">
            <Col xs={7} md={4} xl={3}>
              <Button
                onClick={this.onSignOut}
                className="btn-block"
                data-cy="sign-out"
              >
                Sign Out
              </Button>
            </Col>
          </Row>
          <Row className="m-0 justify-content-center">
            <Col xs={7} md={4} xl={3}>
              <Button
                className="btn-block"
                onClick={this.toggle}
                data-cy="delete-account"
              >
                Delete Account
              </Button>
              <div>
                <Modal isOpen={this.state.isOpen} toggle={this.toggle}>
                  <ModalHeader toggle={this.toggle}>Delete Confirmation</ModalHeader>
                  <ModalBody>
                    Are you sure you want to delete your account?
                  </ModalBody>
                  <ModalFooter>
                    <Button color="primary" data-cy="confirm-delete" onClick={this.onDelete}>Delete</Button>
                    <Button color="secondary" data-cy="cancel-delete" onClick={this.toggle}>Cancel</Button>
                  </ModalFooter>
                </Modal>
              </div>
            </Col>
          </Row>
          {isAdmin && <UserList />}
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

const SettingsPageWithRouter = withRouter(SettingsPage);

export default SettingsPageWithRouter;