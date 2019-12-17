//From https://codeburst.io/adding-google-sign-in-to-your-webapp-a-react-example-dcec8c73cb9f

import React, { Component } from 'react';
import { gapi } from 'gapi-script';

class SignIn extends Component {
  constructor(props) {
    super(props)
    this.state = {
        isSignedIn: false,
		isMock: false
    }
  }

  getContent() {
    if (this.state.isSignedIn || this.state.isMock) {
      return <p>Hello user, you're signed in </p>
    } else {
      return (
        <div>
          <p>You are not signed in. Click here to sign in.</p>
          <button id="loginButton">Login with Google</button>
        </div>
      )
    }   
  }

  onSuccess() {
    this.setState({
      isSignedIn: true,
	  isMock: false
   })
  }

  onFailed(err) {
    this.setState({
      isSignedIn: false,
	  isMock: false
    })
  }

  onInvalidRequest(err) {
    console.log("Site is running locally, using mock data");
	console.log(err);

  	this.setState({
	  isSignedIn: true,
	  isMock: true
	})
  }

  componentDidMount() {
    const successCallback = this.onSuccess.bind(this);
	const failureCallback = this.onFailed.bind(this);
	const invalidCallback = this.onInvalidRequest.bind(this);

    window.gapi.load('auth2', () => {
      this.auth2 = gapi.auth2.init({
        client_id: this.props.clientId,
      });

	  this.auth2.then(() => {}, invalidCallback);

	  if (!this.state.isMock) {
        window.gapi.load('signin2', () => {
          // render a sign in button
          // using this method will show Signed In if the user is already signed in
          var opts = {
            width: 200,
            height: 50,
            onSuccess: successCallback,
		    onFailure: failureCallback
          }
          gapi.signin2.render('loginButton', opts)
        });
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