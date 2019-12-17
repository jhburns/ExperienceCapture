//From https://codeburst.io/adding-google-sign-in-to-your-webapp-a-react-example-dcec8c73cb9f

import React, { Component } from 'react';
import { gapi } from 'gapi-script';

import { submitUser } from "libs/userManagement";

import SignOutButton from "components/SignOutButton"

class SignIn extends Component {
  constructor(props) {
    super(props)
    this.state = {
        isSignedIn: false,
		isMock: false,
		isUnableToSignIn: false,
		user: null
    }
  }

  getContent() {
    if (this.state.isSignedIn) {
      return (
		<div>
		  <p>You're Signed In</p>
		  <SignOutButton />
		</div>
	  )
	} else if (this.state.isUnableToSignIn) {
	  return <p>Sorry, there is an issue signing in.</p>
    } else {
      return (
        <div>
          <p>Please Sign In</p>
          <button id="loginButton">Login with Google</button>
        </div>
      )
    }   
  }

  onSuccess(user) {
	submitUser(user, undefined, this.onFailure);

    this.setState({
      isSignedIn: true,
	  user
    })
  }

  onFailure() {
    console.log("Error signing user in.");

    this.setState({
	  isUnableToSignIn: true
    })
  }

  onInvalidRequest(err) {
    console.log("Site is running locally, using mock data");
	console.log(err);

  	this.setState({
	  isSignedIn: true,
	  isMock: true,
	})
  }

  componentDidMount() {
    const successCallback = this.onSuccess.bind(this);
	const failureCallback = this.onFailure.bind(this);
	const invalidCallback = this.onInvalidRequest.bind(this);

    window.gapi.load('auth2', () => {
      this.auth2 = gapi.auth2.init({
        client_id: this.props.clientId,
      });

	  this.auth2.then(() => {}, invalidCallback);

	  if (!this.state.isMock) {
        window.gapi.load('signin2', () => {
          var opts = {
            width: 200,
            height: 50,
            onsuccess: successCallback,
		    onfailure: failureCallback
          }
          gapi.signin2.render('loginButton', opts)
        });
	  } else {
	    submitUser(null, true, failureCallback);
	  }
    })
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