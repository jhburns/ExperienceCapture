import React, { Component } from 'react';
import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';

class Admin extends Component {
  render() {
    return (
      <div>
        <p>Redirecting...</p>
        <p>Unless password is invalid</p>
      </div>
    );
  }

  async componentDidMount() {
    const query = queryString.parse(this.props.location.search);
    console.log(query.password);

    try {
      const data = {
        password: query.password
      };

      const reply = await postData("/api/v1/users/signUp/admin/", data);

      if (reply.ok) {
        console.log(await reply.text());
      } else {
        throw Error(reply.error);
      }
    } catch (error) {
      console.error(error);
    }
  }
}

export default Admin;
