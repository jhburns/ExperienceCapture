import React, { Component } from 'react';

import moment from 'moment';

class Session extends Component {
  render() {
    return (
      <tr>
        <th scope="row">{this.props.sessionData.id}</th>
        <td>{this.props.sessionData.fullname}</td>
        <td>{moment(this.props.sessionData.createdAt).fromNow()}</td>
        {this.props.buttonData !== undefined &&
          <td>
            <button onClick={() => this.props.buttonData.onClick(this.props.sessionData.id)}>
              {this.props.buttonData.body}
            </button>
          </td>
        }
      </tr>
    )
  }
}

export default Session;