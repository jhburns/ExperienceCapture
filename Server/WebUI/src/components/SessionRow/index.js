import React, { Component } from 'react';

import moment from 'moment';
import { Link } from 'react-router-dom';

class Session extends Component {
  render() {
    return (
      <tr>
          <th scope="row">
            <Link to={`/home/sessions/${this.props.sessionData.id}/`}>
              {this.props.sessionData.id}
            </Link>
          </th>

        <td>{this.props.sessionData.fullname}</td>
        <td>{
            this.props.isRenderingDate ? 
              moment(this.props.sessionData.createdAt).format("MMM Do YY hh:mm a") :
              moment(this.props.sessionData.createdAt).fromNow()
        }</td>
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