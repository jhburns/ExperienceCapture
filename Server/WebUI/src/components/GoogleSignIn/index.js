import React, { Component } from 'react';
import { gapi } from 'gapi-script';

import { submitUser, signOutUser, } from "libs/userManagement";
import SignOutButton from "components/SignOutButton"

import HomeButton from 'components/HomeButton';

import { Wrapper } from 'components/GoogleSignIn/style';

import { P, Button, } from '@bootstrap-styled/v4';

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
          <div>
            <P>Sorry, there was an issue signing in.</P>
            <P>Try a different account.</P>
            <SignOutButton onClickCallback={this.signOutCallback} />
          </div>
        </Wrapper>
      )
    } else if (this.state.isDuplicateSignIn) {
      return (
        <Wrapper>
          <div>
            <P>You've Already Signed Up</P>
            <HomeButton />
            <SignOutButton onClickCallback={this.signOutCallback} />
          </div>
        </Wrapper>
	    )
    } else if (this.state.isSignedIn) {
      return (
        <Wrapper>
          <div>
            <P>You're Signed In</P>
            <HomeButton />
            <SignOutButton onClickCallback={this.signOutCallback} />
          </div>
        </Wrapper>
	    )
	  } else if (this.state.isSignedOut) {
	    return (
        <Wrapper>
          <div>
            <P>You're Signed Out</P>
            <P>Sign In Again</P>
            <Button id="loginButton">Sign In With Google</Button>
          </div>
        </Wrapper>
	    )
    } else {
      return (
        <Wrapper>
          <div>
            <P>Please Sign In</P>
            <Button id="loginButton">Sign In With Google</Button>
          </div>
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