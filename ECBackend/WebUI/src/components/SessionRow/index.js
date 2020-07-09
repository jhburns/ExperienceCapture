import React, { Component } from 'react';

import moment from 'moment';
import { Link } from 'react-router-dom';

import { Wrapper } from 'components/SessionRow/style';

class SessionRow extends Component {
  render() {
    return (
      <Wrapper>
        <th scope="row" data-cy="session-row">
          <Link
            to={`/home/sessions/id/${this.props.sessionData.id}/`}
            data-cy="session-link"
          >
            {this.props.sessionData.id}
          </Link>
        </th>
        <td>{this.props.sessionData.fullname}</td>
        <td data-cy="session-date">{
          this.props.isRenderingDate ?
            moment(this.props.sessionData.createdAt).format("MMM Do, YYYY, h:mm A") :
            moment(this.props.sessionData.createdAt).fromNow()
        }</td>
        {this.props.buttonData !== undefined &&
          <td>
            <button
              onClick={() => this.props.buttonData.onClick(this.props.sessionData.id)}
              className="btn btn-outline-dark mr-2 mr-lg-0"
              data-cy={`session-button-${this.props.sessionData.id}`}
            >
              {this.props.buttonData.body}
            </button>
          </td>
        }
      </Wrapper>
    )
  }
}

export default SessionRow;