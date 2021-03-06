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

import { withRouter, Switch, Route } from "react-router";

import NormalSignInPage from "pages/SignIn";

class ErrorBoundary extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isHandling: false,
    };

    this.onSignOut = this.onSignOut.bind(this);
  }

  async onSignOut() {
    const goToRoot = () => {
      const { history } = this.props;
      history.push('/');

      this.setState({ isHandling: false });
    };

    try {
      signOutUser(undefined, () => { goToRoot(); }, () => { goToRoot(); });
    } catch (ignore) {
      // Ignore because we are already handling another error.
    }
  }

  componentDidCatch(error, info) {
    this.setState({ isHandling: true });
  }

  render() {
    if (this.state.isHandling) {
      return (
        <Wrapper>
          <Switch>
            <Route exact path="/" component={NormalSignInPage} />
          </Switch>
          <Menu />
          <Container className="p-0">
            <Modal isOpen={true}>
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
          </Container>
          <Footer />
        </Wrapper>
      );
    }

    return this.props.children;
  }
}

const ErrorBoundaryWithRouter = withRouter(ErrorBoundary);

export default ErrorBoundaryWithRouter;