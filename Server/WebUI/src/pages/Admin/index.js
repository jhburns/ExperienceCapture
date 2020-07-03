import React, { Component } from 'react';
import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';
import { Redirect } from 'react-router-dom';

import { Wrapper } from 'pages/Admin/style';

class Admin extends Component {

  constructor(props) {
    super(props)
    this.state = {
      isError: false,
      isWaiting: true,
      accessToken: null,
    }

    this.onSuccess = this.onSuccess.bind(this);
    this.onError = this.onError.bind(this);
  }

  onSuccess(response) {
    this.setState({
      accessToken: response.signUpToken,
      isWaiting: false
    });
  }
  
  onError() {
    this.setState({
      isError: true,
      isWaiting: false
    });
  }

  async componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    try {
      const data = {
        password: query.password
      };

      const reply = await postData("/api/v1/authorization/admin/", data);

      if (reply.ok) {
        this.onSuccess(await reply.json());
      } else {
        this.onError();
        throw Error(reply.error);
      }
    } catch (error) {
      this.onError();
      console.error(error);
    }
  }

  render() {
    if (this.state.isWaiting) {
      return (
        <Wrapper>
          <p>One sec...</p>
        </Wrapper>
      )
    } else if (this.state.isError) {
      return (
        <Wrapper>
          <p>Password is Invalid</p>
          <p>Check console</p>
        </Wrapper>
      )
    } else {
      return (
        <Wrapper>
          <p>Redirecting...</p>
          <Redirect
            to={{
              pathname: "/signUp",
              search: queryString.stringify({signUpToken: this.state.accessToken}),
            }}
          />
        </Wrapper>
      )
    }
  }
}

export default Admin;
