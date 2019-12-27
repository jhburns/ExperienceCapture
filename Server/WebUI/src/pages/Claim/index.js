import React, { Component } from 'react';
import logo from 'logo.svg';

import GoogleSignIn from "components/GoogleSignIn";

import queryString from 'query-string';

class ClaimPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
        claimToken: null,
    }
  }

  componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    this.setState({
        claimToken: query.claimToken
    });
  }

  render() {
    return (
      <div>
        <img src={logo} className="App-logo" alt="logo" />
	      <GoogleSignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} claimToken={this.state.claimToken}/>
      </div>
    );
  }
}

export default ClaimPage;
