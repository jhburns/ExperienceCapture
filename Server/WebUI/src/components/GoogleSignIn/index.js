import React, { Component } from 'react';
import { gapi } from 'gapi-script';

import { submitUser, signOutUser, } from "libs/userManagement";
import SignOutButton from "components/SignOutButton"

import HomeButton from 'components/HomeButton';

import { Wrapper, Info, Google } from 'components/GoogleSignIn/style';

import { P, Button, Row, Col, } from '@bootstrap-styled/v4';

class SignIn extends Component {
  constructor(props) {
    super(props)
    this.state = {
      isSignedIn: false,
	    isSignedOut: false, 
	    isMock: false,
      isUnableToSignIn: false,
	    isDuplicateSignIn: false,
    }

	  this.successCallback = this.onSuccess.bind(this);
	  this.failureCallback = this.onFailure.bind(this);
	  this.invalidCallback = this.onInvalidRequest.bind(this);
	  this.signOutCallback = this.onSignOut.bind(this);
    this.renderLoginCallback = this.renderLogin.bind(this);
    this.duplicateCallback = this.onDuplicate.bind(this);
  }

  getContent() {
  	if (this.state.isUnableToSignIn) {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  Sorry, there was an issue signing in <br />
                  Try a different account
                </h5>
              </Info>
            </Col>
          </Row>
          <Row>
            <Col>
              <SignOutButton onClickCallback={this.signOutCallback} />
            </Col>
          </Row>
        </Wrapper>
      )
    } else if (this.state.isDuplicateSignIn) {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  You've Already Signed Up
                </h5>
              </Info>
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6} className="mb-2">
              <HomeButton />
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6}>
              <SignOutButton onClickCallback={this.signOutCallback} />
            </Col>
          </Row>
        </Wrapper>
	    )
    } else if (this.state.isSignedIn) {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  You're Signed In
                </h5>
              </Info>
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6} className="mb-2">
              <HomeButton />
            </Col>        
          </Row>
          <Row className="justify-content-center">
            <Col xs={6}>
              <SignOutButton onClickCallback={this.signOutCallback} />
            </Col>
          </Row>
        </Wrapper>
	    )
	  } else if (this.state.isSignedOut) {
	    return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} className="mb-4">
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
              <Google id="loginButton">Sign In With Google</Google>
            </Col>
          </Row>
        </Wrapper>
	    )
    } else {
      return (
        <Wrapper>
          <Row className="justify-content-center">
            <Col xs={10} className="mb-4">
              <Info className="rounded align-middle">
                <h5 className="mt-0 mb-0">
                  <P>Please Sign In</P>
                </h5>
              </Info>
            </Col>
          </Row>
          <Row>
            <Col className="text-center">
              <Google id="loginButton">Sign In With Google</Google>
            </Col>
          </Row>
        </Wrapper>
      )
    }   
  }

  renderLogin() {
    const opts = {
      width: 220,
      height: 50,
	    longtitle: true,
      onsuccess: this.successCallback,
      onfailure: this.failureCallback
    }
    
	  gapi.signin2.render('loginButton', opts);
  }

  onDuplicate() {
    this.setState({
      isDuplicateSignIn: true,
      isSignedIn: true,
    });
  }

  async onSignOut() {
    await signOutUser(this.state.isMock);
    this.setState({
	    isSignedIn: false,
      isSignedOut: true,
      isDuplicateSignIn: false,
      isUnableToSignIn: false,
    }, () => window.gapi.load('signin2', this.renderLoginCallback));
  }

  async onSuccess(user) {
    const options = {
      signUpToken: this.props.signUpToken,
      claimToken: this.props.claimToken,
    };
	  await submitUser(undefined, user, this.failureCallback, options, this.duplicateCallback);

    this.setState({
      isSignedIn: true,
    });
  }

  onFailure() {
    console.log("Error signing user in.");

    this.setState({
	    isUnableToSignIn: true
    });
  }

  onInvalidRequest(err) {
    console.log("Site is running locally, using mock data");
    console.log(err);

    const options = {
      signUpToken: this.props.signUpToken,
      claimToken: this.props.claimToken,
    };
    submitUser(true, null, this.failureCallback, options, this.duplicateCallback);
    
  	this.setState({
	    isSignedIn: true,
	    isMock: true,
	  });
  }

  componentDidMount() {
    window.gapi.load('auth2', () => {
      this.auth2 = gapi.auth2.init({
        client_id: this.props.clientId,
      });

	    this.auth2.then(() => {}, this.invalidCallback);
      window.gapi.load('signin2', this.renderLoginCallback);
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