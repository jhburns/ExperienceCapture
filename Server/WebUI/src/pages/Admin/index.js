import React, { Component } from 'react';
import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';
import { BrowserRouter as Router, Redirect } from 'react-router-dom';

class Admin extends Component {

  constructor(props) {
    super(props)
    this.state = {
      isError: false,
      accessToken: null
    }

    this.successCallback = this.onSuccess.bind(this);
	  this.errorCallback = this.onError.bind(this);
  }

  onSuccess(token) {
    this.setState({
      accessToken: token
    });
  }
  
  onError() {
    this.setState({
      isError: true
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
    if (this.state.isError) {
      return (
        <div>
          <p>Password is Invalid</p>
          <p>Check console</p>
        </div>
      );
    } else {
      return (
        <div>
          <p>Redirecting...</p>
          <Router>
            <Redirect
              to={{
                pathname: "/signUp",
                search: queryString.stringify({signUpToken: this.state.accessToken}),
              }}
            />
          </Router>
        </div>
      );
    }
  }
}

export default Admin;
