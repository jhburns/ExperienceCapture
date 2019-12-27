import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import queryString from 'query-string';

class SignUpPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      signUpToken: null,
    }
  }

  componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    this.setState({
      signUpToken: query.signUpToken
    });
  }

  render() {
    return (
      <div>
        <img src={logo} className="App-logo" alt="logo" />
	      <GoogleSignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} signUpToken={this.state.signUpToken}/>
      </div>
    );
  }
}

export default SignUpPage;
