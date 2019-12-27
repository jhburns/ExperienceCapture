import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import queryString from 'query-string';

class NormalSignInPage extends Component {
  render() {
    return (
      <div>
        <img src={logo} className="App-logo" alt="logo" />
	      <GoogleSignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} />
      </div>
    );
  }
}

export default NormalSignInPage;
