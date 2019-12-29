import React, { Component } from 'react';

import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';

class GetSignUpLink extends Component {
  constructor(props) {
    super(props);

    this.state = {
      link: ""
    };

    this.buttonCallback = this.onButtonCLick.bind(this);
  }

  async onButtonCLick() {
    const signUpRequest = await postData("/api/v1/users/signUp/", {});

    if (!signUpRequest.ok) {
      throw new Error(signUpRequest.status);
    }

    const signUpToken = await signUpRequest.text();
    const query = queryString.stringify({ signUpToken: signUpToken });

    this.setState({
      link: `https://${window.location.host}/signUp?${query}` // Remove https manually when working locally
    });
  }

  render() {
    return (
      <div>
        <p>{this.state.link}</p>
        <button onClick={this.buttonCallback}>New Sign Up Link</button>
      </div>
    );
  }
}
export default GetSignUpLink;