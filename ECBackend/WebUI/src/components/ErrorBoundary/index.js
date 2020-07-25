// From: https://reactjs.org/blog/2017/07/26/error-handling-in-react-16.html

import React, { Component } from 'react';

import { Wrapper } from 'components/ErrorBoundary/style';
import {
  Container,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  P,
} from '@bootstrap-styled/v4';

import Menu from 'components/Menu';
import Footer from "components/Footer";

import { signOutUser } from 'libs/userManagement';

import { withRouter, Switch, Route, } from "react-router";

import NormalSignInPage from "pages/NormalSignIn";

class ErrorBoundary extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isHandling: false
    };

    this.onSignOut = this.onSignOut.bind(this);
  }

  async onSignOut() {
    try {
      await signOutUser(undefined);
    } catch (ignore) {
      // Ignore because we are already handling another error.
    }

    const { history } = this.props;
    history.push('/');

    this.setState({ isHandling: false });
  }

  componentDidCatch(error, info) {
    this.setState({ isHandling: true });

    console.error(`The following error is handled by the ErrorBoundary component: ${error}\n ${info}`);
  }

  render() {
    if (this.state.isHandling) {
      return (
        <Wrapper>
          <Switch>
            <Route exact path="/" component={NormalSignInPage} />
          </Switch>
          <Container className="p-0">
            <Menu />
            <Modal isOpen={true} documentClassName="modal-dialog-centered">
              <ModalHeader >Sorry, something went wrong.</ModalHeader>
              <ModalBody>
                <P>
                  Please sign out and then back in, which may fix the issue.
                </P>
                <P>
                  If the problem persists, contact your admin.
                </P>
              </ModalBody>
              <ModalFooter>
                <Button color="secondary" onClick={this.onSignOut}>Sign Out</Button>
              </ModalFooter>
            </Modal>
            <Footer />
          </Container>
        </Wrapper>
      );
    }

    return this.props.children;
  }
}

const ErrorBoundaryWithRouter = withRouter(ErrorBoundary);

export default ErrorBoundaryWithRouter;