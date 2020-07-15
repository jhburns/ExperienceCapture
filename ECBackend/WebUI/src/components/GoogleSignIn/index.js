/* global gapi */

import React, { Component } from 'react';

import { submitUser, signOutUser, } from "libs/userManagement";
import ClaimNotify from "components/ClaimNotify";

import { Wrapper, Google, } from 'components/GoogleSignIn/style';

import { Row, Col, Button } from '@bootstrap-styled/v4';

import LoginBox from 'components/LoginBox';

import { Link } from "react-router-dom";

class SignIn extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isSignedIn: false,
	    isSignedOut: false, 
	    isMock: false,
      isUnableToSignIn: false,
	    isDuplicateSignUp: false,
    }

    this.onSuccess = this.onSuccess.bind(this);
    this.onFailure = this.onFailure.bind(this);
    this.onInvalidRequest = this.onInvalidRequest.bind(this);
    this.onSignOut = this.onSignOut.bind(this);
    this.renderLogin = this.renderLogin.bind(this);
    this.onDuplicate = this.onDuplicate.bind(this);
    this.getContent = this.getContent.bind(this);
  }

  getContent() {
    const signOutRow =
      <Row className="justify-content-center">
        <Col xs={6} sm={5} md={4} lg={3} className="mb-2">
          <Button
            onClick={this.onSignOut}
            className="btn btn-outline-dark btn-block"
            data-cy="sign-out"
          >
            Sign Out
          </Button>
        </Col>
      </Row>;

    const homeRow =
      <Row className="justify-content-center">
        <Col xs={6} sm={5} md={4} lg={3} className="mb-2">
          <Link to="/home/start" className="btn btn-dark btn-block" data-cy="go-home">
            Go Home
          </Link>
        </Col>
      </Row>;

    const googleRow =
      <Row>
        <Col className="text-center">
          <Google id="loginButton" />
        </Col>
      </Row>;

  	if (this.state.isUnableToSignIn) {
      return (
        <Wrapper>
          <LoginBox>
            Sorry, there was an issue signing in. <br />
            Try a different account.
          </LoginBox>
          {signOutRow}
        </Wrapper>
      )
    } else if (this.state.isDuplicateSignUp) {
      return (
        <Wrapper data-cy="already-notify">
          <LoginBox>
            You're Already Signed Up.
          </LoginBox>
          {homeRow}
          {signOutRow}
        </Wrapper>
	    )
    } else if (this.state.isSignedIn) {
      if (this.props.claimToken === undefined) {
        return (
          <Wrapper>
            <LoginBox>
              You're Signed In.
            </LoginBox>
            {homeRow}
            {signOutRow}
          </Wrapper>
        )
      } else {
        return (
          <ClaimNotify />
        )
      }
	  } else if (this.state.isSignedOut) {
	    return (
        <Wrapper>
          <LoginBox>
            You're Signed Out. <br />
            Sign In Again.
          </LoginBox>
          {googleRow}
        </Wrapper>
	    )
    } else {
      return (
        <Wrapper>
          <LoginBox>
            Please Sign In.
          </LoginBox>
          {googleRow}
        </Wrapper>
      )
    }   
  }

  renderLogin(isMock) {
    const opts = {
      width: 220,
      height: 50,
	    longtitle: true,
      onsuccess: this.onSuccess,
      onfailure: this.onFailure
    }
    
	  if (!isMock) {
      gapi.signin2.render('loginButton', opts);
    }
  }

  onDuplicate() {
    this.setState({
      isDuplicateSignUp: true,
      isSignedIn: true,
    });
  }

  async onSignOut() {
    const isMock = this.state.isMock;

    await signOutUser(isMock);
    this.setState({
	    isSignedIn: false,
      isSignedOut: true,
      isDuplicateSignUp: false,
      isUnableToSignIn: false,
    }, () => gapi.load('signin2', this.renderLogin(isMock)));
  }

  async onSuccess(user) {
    const options = {
      signUpToken: this.props.signUpToken,
      claimToken: this.props.claimToken,
    };
	  await submitUser(undefined, user, this.onFailure, options, this.onDuplicate);

    this.setState({
      isSignedIn: true,
    });
  }

  onFailure() {
    this.setState({
	    isUnableToSignIn: true
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
    gapi.load('auth2', () => {
      this.auth2 = gapi.auth2.init({
        client_id: this.props.clientId,
      });

      this.auth2.then(() => gapi.load('signin2', this.renderLogin), this.onInvalidRequest);
    });
  }

  render() {
    return (
	    <div className="SignIn">
		    {this.getContent()}
      </div>
    )
  }
}

export default SignIn;