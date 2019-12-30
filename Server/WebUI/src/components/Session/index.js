import React, { Component } from 'react';

import moment from 'moment';

class Session extends Component {
  render() {
    return (
      <tr>
        <th scope="row">{this.props.sessionData.id}</th>
        <td>{this.props.sessionData.fullname}</td>
        <td>{moment(this.props.sessionData.createdAt).fromNow()}</td>
      </tr>
    )
  }
}

export default Session;