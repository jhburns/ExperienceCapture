import React, { Component } from 'react';

import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';

import { Text, Wrapper } from "components/GetSignUpLink/style";

import { Button } from '@bootstrap-styled/v4';


class GetSignUpLink extends Component {
  constructor(props) {
    super(props);

    this.state = {
      link: "",
      error: null,
    };

    this.onButtonCLick = this.onButtonCLick.bind(this);
  }

  async onButtonCLick() {
    const signUpRequest = await postData("/api/v1/authentication/signUps/", {});

    if (!signUpRequest.ok) {
      this.setState({ error: new Error(signUpRequest.status) });
    }

    const request = await signUpRequest.json();
    const query = queryString.stringify({ signUpToken: request.signUpToken });
    const source = window.location.origin;

    this.setState({
      link: `${source}/?${query}`,
    });
  }

  render() {
    if (this.state.error) {
      throw this.state.error;
    }

    return (
      <Wrapper>
        {this.state.link !== "" &&
          <Text className="mt-4 mb-4" data-cy="sign-up-link">
            {this.state.link}
          </Text>
        }
        <Button onClick={this.onButtonCLick} data-cy="new-sign-up" size="lg">
          New Sign Up Link
        </Button>
      </Wrapper>
    );
  }
}
export default GetSignUpLink;