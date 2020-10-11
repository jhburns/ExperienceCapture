/* global gapi */

import React, { Component } from 'react';

import { submitUser, signOutUser, gapiSetup } from "libs/userManagement";

import { Wrapper } from 'components/GoogleSignIn/style';

import { Row, Col, Button } from '@bootstrap-styled/v4';

import { LinkContainer } from 'react-router-bootstrap';

import SignInBox from 'components/SignInBox';

class SignIn extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isSignedIn: false,
	    isSignedOut: false,
	    isMock: false,
      isUnableToSignIn: false,
      isDuplicateSignUp: false,
      error: null,
    };

    this.onSuccess = this.onSuccess.bind(this);
    this.onFailure = this.onFailure.bind(this);
    this.onInvalidRequest = this.onInvalidRequest.bind(this);
    this.onSignOut = this.onSignOut.bind(this);
    this.attachLogin = this.attachLogin.bind(this);
    this.onDuplicate = this.onDuplicate.bind(this);
    this.getContent = this.getContent.bind(this);
  }

  getContent() {
    if (this.state.error) {
      throw this.state.error;
    }

    const homeRow =
      <Col xs={8} lg={6} className="mb-2 lb-lg-0">
        <LinkContainer to="/home/start" >
          <Button
            data-cy="go-home"
            block={true}
            className="text-decoration-none"
            size="lg"
          >
            GO HOME
          </Button>
        </LinkContainer>
      </Col>;

    const signOutRow =
      <Col xs={8} lg={6} className="mb-2 mb-lg-0">
        <Button
          onClick={this.onSignOut}
          data-cy="sign-out"
          block={true}
          outline={true}
          size="lg"
        >
          SIGN OUT
        </Button>
      </Col>;

    const googleRow =
      <Col xs={8} lg={7} className="mb-2 mb-lg-0">
        <Button
          id="signInButton"
          block={true}
          size="lg"
          data-cy="sign-in"
        >
          GOOGLE SIGN IN
        </Button>
      </Col>;

  	if (this.state.isUnableToSignIn) {
      return (
        <Wrapper>
          <SignInBox>
            Sorry, there was an issue signing in. <br />
            Try again.
          </SignInBox>
          <Row className="justify-content-center">
            {signOutRow}
          </Row>
        </Wrapper>
      );
    } else if (this.state.isDuplicateSignUp) {
      return (
        <Wrapper data-cy="already-notify">
          <SignInBox>
            You've Already Signed Up.
          </SignInBox>
          <Row className="justify-content-center">
            {homeRow}
            {signOutRow}
          </Row>
        </Wrapper>
	    );
    } else if (this.state.isSignedIn) {
      if (this.props.claimToken === undefined) {
        return (
          <Wrapper>
            <SignInBox>
              You're Signed In.
            </SignInBox>
            <Row className="justify-content-center">
              {homeRow}
              {signOutRow}
            </Row>
          </Wrapper>
        );
      } else {
        this.props.onSuccessfulClaim();
        return (
          <Wrapper />
        );
      }
	  } else if (this.state.isSignedOut) {
	    return (
        <Wrapper>
          <SignInBox>
            You've Signed Out.
          </SignInBox>
          <Row className="justify-content-center">
            {googleRow}
          </Row>
        </Wrapper>
	    );
    } else {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            {googleRow}
          </Row>
        </Wrapper>
      );
    }
  }

  attachLogin(isMock = false) {
    const signInButton = document.getElementById('signInButton');

    if (isMock) {
      signInButton.onclick = () => console.log("This button does nothing when using mock data.");
      return;
    }

    try {
      let auth2 = gapi.auth2.getAuthInstance();

      auth2.attachClickHandler(signInButton, {},
        (user) => this.onSuccess(user),
        (err) => this.setState({ error: err }),
      );
    } catch (err) {
      this.setState({ error: err });
    }
  }

  onDuplicate() {
    this.setState({
      isDuplicateSignUp: true,
      isSignedIn: true,
    });
  }

  async onSignOut() {
    signOutUser(() => {
      this.setState({
        isSignedIn: false,
        isSignedOut: true,
        isDuplicateSignUp: false,
        isUnableToSignIn: false,
      }, () => gapi.load('signin2', this.attachLogin(this.state.isMock)));
    }, (err) => this.setState({ error: err }));
  }

  async onSuccess(user) {
    const options = {
      signUpToken: this.props.signUpToken,
      claimToken: this.props.claimToken,
    };

    try {
      await submitUser(undefined, user, this.onFailure, options, this.onDuplicate);

      this.setState({
        isSignedIn: true,
      });
    } catch (err) {
      this.setState({ error: err });
    }
  }

  onFailure() {
    this.setState({
	    isUnableToSignIn: true,
    });
  }

  async onInvalidRequest(err) {
    console.log("Site is running locally, using mock data. See printed error.");

    const options = {
      signUpToken: this.props.signUpToken,
      claimToken: this.props.claimToken,
    };
    await submitUser(true, null, this.onFailure, options, this.onDuplicate);

  	this.setState({
	    isSignedIn: true,
	    isMock: true,
    });

    console.log(err);
  }

  componentDidMount() {
    gapiSetup(() => gapi.load('signin2', this.attachLogin), this.onInvalidRequest);
  }

  render() {
    return this.getContent();
  }
}

export default SignIn;