import React, { Component } from 'react';

import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';

import { Text, Wrapper, } from "components/GetSignUpLink/style"

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
      <Wrapper>
        {this.state.link !== "" &&
          <Text className="mt-4 mb-4">
            {this.state.link}
          </Text>
        }
        <button
          onClick={this.buttonCallback}
          className="btn btn-outline-dark btn-block"
        >
          New Sign Up Link
        </button>
      </Wrapper>
    );
  }
}
export default GetSignUpLink;