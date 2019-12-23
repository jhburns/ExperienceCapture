import React from 'react';
import logo from 'logo.svg';

import SignIn from "components/SignIn";

function SignUpPage() {
  return (
    <div>
        <img src={logo} className="App-logo" alt="logo" />
	    <SignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} />
    </div>
  );
};

export default SignUpPage;
