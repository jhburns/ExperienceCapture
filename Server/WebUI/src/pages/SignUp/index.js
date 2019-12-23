import React from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

function SignUpPage() {
  return (
    <div>
      <img src={logo} className="App-logo" alt="logo" />
	    <GoogleSignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} />
    </div>
  );
}

export default SignUpPage;
