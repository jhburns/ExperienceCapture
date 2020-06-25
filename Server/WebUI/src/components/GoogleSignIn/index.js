/* global gapi */

import React, { Component } from 'react';

import { submitUser, signOutUser, } from "libs/userManagement";
import SignOutButton from "components/SignOutButton"
import ClaimNotify from "components/ClaimNotify";

import HomeButton from 'components/HomeButton';

import { Wrapper, Info, Google, } from 'components/GoogleSignIn/style';

import { P, Row, Col, } from '@bootstrap-styled/v4';

// TODO: Consider using a state string to model sign-in
class SignIn extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isSignedIn: false,
	    isSignedOut: false, 
	    isMock: false,
      isUnableToSignIn: false,
	    isDuplicateSignIn: false,
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
    // TODO: refactor to a reusable architecture
  	if (this.state.isUnableToSignIn) {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} sm={8} md={6} lg={4} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  Sorry, there was an issue signing in. <br />
                  Try a different account.
                </h5>
              </Info>
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6} sm={5} md={4} lg={3} className="mb-2">
              <SignOutButton onClickCallback={this.onSignOut} />
            </Col>
          </Row>
        </Wrapper>
      )
    } else if (this.state.isDuplicateSignIn) {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} sm={8} md={6} lg={4} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  You're Already Signed Up
                </h5>
              </Info>
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6} sm={5} md={4} lg={3} className="mb-2">
              <HomeButton />
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6} sm={5} md={4} lg={3}>
              <SignOutButton onClickCallback={this.onSignOut} />
            </Col>
          </Row>
        </Wrapper>
	    )
    } else if (this.state.isSignedIn) {
      if (this.props.claimToken === undefined) {
        return (
          <Wrapper>
            <Row className="justify-content-center">
              <Col xs={10} sm={8} md={6} lg={4} className="mb-4">
                <Info className="rounded align-middle">
                  <h5 className="mt-0 mb-0">
                    You're Signed In
                  </h5>
                </Info>
              </Col>
            </Row>
            <Row className="justify-content-center">
              <Col xs={6} sm={5} md={4} lg={3} className="mb-2">
                <HomeButton />
              </Col>
            </Row>
            <Row className="justify-content-center">
              <Col xs={6} sm={5} md={4} lg={3}>
                <SignOutButton onClickCallback={this.onSignOut} />
              </Col>
            </Row>
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
          <Row className="justify-content-center">
            <Col xs={10} sm={8} md={6} lg={4} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  You're Signed Out <br />
                  Sign In Again
               </h5>
              </Info>
            </Col>
          </Row>
          <Row>
            <Col className="text-center">
              <Google id="loginButton" />
            </Col>
          </Row>
        </Wrapper>
	    )
    } else {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} sm={8} md={6} lg={4} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  <P>Please Sign In</P>
                </h5>
              </Info>
            </Col>
          </Row>
          <Row>
            <Col className="text-center">
              <Google id="loginButton" />
            </Col>
          </Row>
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
      isDuplicateSignIn: true,
      isSignedIn: true,
    });
  }

  async onSignOut() {
    const isMock = this.state.isMock;

    await signOutUser(isMock);
    this.setState({
	    isSignedIn: false,
      isSignedOut: true,
      isDuplicateSignIn: false,
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