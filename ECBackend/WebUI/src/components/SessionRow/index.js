import React, { Component } from 'react';

import { DateTime } from 'luxon';

import { Wrapper } from 'components/SessionRow/style';

import { A, Th, Td, Button } from '@bootstrap-styled/v4';

import { LinkContainer } from 'react-router-bootstrap';

class SessionRow extends Component {
  render() {
    return (
      <Wrapper>
        <Th scope="row" data-cy="session-row">
          <LinkContainer
            to={`/home/sessions/id/${this.props.sessionData.id}/`}
            data-cy="session-link"
          >
            <A className="font-weight-normal">
              {this.props.sessionData.id}
            </A>
          </LinkContainer>
        </Th>
        <Td>{this.props.sessionData.fullname}</Td>
        <Td data-cy="session-date">{DateTime.fromMillis(this.props.sessionData.createdAt).toRelative()}</Td>
        {this.props.buttonData !== undefined &&
          <Td>
            <Button
              onClick={() => this.props.buttonData.onClick(this.props.sessionData.id)}
              className="btn btn-outline-dark mr-2 mr-lg-0"
              data-cy={`session-button`}
            >
              {this.props.buttonData.body}
            </Button>
          </Td>
        }
      </Wrapper>
    );
  }
}

export default SessionRow;