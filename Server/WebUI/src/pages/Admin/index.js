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

    this.successCallback = this.onSuccess.bind(this);
	  this.errorCallback = this.onError.bind(this);
  }

  onSuccess(token) {
    this.setState({
      accessToken: token,
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

      const reply = await postData("/api/v1/users/signUp/admin/", data);

      if (reply.ok) {
        this.successCallback(await reply.text());
      } else {
        this.errorCallback();
        throw Error(reply.error);
      }
    } catch (error) {
      this.errorCallback();
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
