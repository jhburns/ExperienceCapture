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

    this.onButtonCLick = this.onButtonCLick.bind(this);
  }

  async onButtonCLick() {
    const signUpRequest = await postData("/api/v1/users/authentication/signUps/", {});

    if (!signUpRequest.ok) {
      throw new Error(signUpRequest.status);
    }

    const request = await signUpRequest.json();
    const query = queryString.stringify({ signUpToken: request.signUpToken });
    const source = window.location.origin;

    this.setState({
      link: `${source}/signUp?${query}`
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
          onClick={this.onButtonCLick}
          className="btn btn-dark btn-block"
        >
          New Sign Up Link
        </button>
      </Wrapper>
    );
  }
}
export default GetSignUpLink;