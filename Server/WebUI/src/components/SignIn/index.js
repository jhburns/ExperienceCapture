import React, { Component } from 'react';
import { gapi } from 'gapi-script';

import { submitUser, signOutUser } from "libs/userManagement";
import SignOutButton from "components/SignOutButton"

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
        <div>
          <p>Sorry, there was an issue signing in.</p>
        </div>
      )
    } else if (this.state.isDuplicateSignIn) {
      return (
		    <div>
		      <p>You've Already Signed Up</p>
          <SignOutButton onClickCallback={this.signOutCallback} />
		    </div>
	    )
    } else if (this.state.isSignedIn) {
      return (
		    <div>
		      <p>You're Signed In</p>
		        <SignOutButton onClickCallback={this.signOutCallback} />
		    </div>
	    )
	  } else if (this.state.isSignedOut) {
	    return (
		    <div>
		      <p>You're Signed Out</p>
		      <p>Sign In Again</p>
          <button id="loginButton">Sign In With Google</button>
		    </div>
	    )
    } else {
      return (
        <div>
          <p>Please Sign In</p>
          <button id="loginButton">Sign In With Google</button>
        </div>
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
    }, () => window.gapi.load('signin2', this.renderLoginCallback));
  }

  onSuccess(user) {
	  submitUser(undefined, user, this.failureCallback, this.duplicateCallback);

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
    submitUser(true, null, this.failureCallback, this.duplicateCallback)

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

  componentDidUpdate(prevProps, prevState) {
    Object.entries(this.props).forEach(([key, val]) =>
      prevProps[key] !== val && console.log(`Prop '${key}' changed`)
    );
    if (this.state) {
      Object.entries(this.state).forEach(([key, val]) =>
        prevState[key] !== val && console.log(`State '${key}' changed`)
      );
    }
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