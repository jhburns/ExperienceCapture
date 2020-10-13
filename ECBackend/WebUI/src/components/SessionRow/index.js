import React, { Component } from 'react';

import { DateTime } from 'luxon';

import { Wrapper } from 'components/SessionRow/style';

import { A, Row, Col } from '@bootstrap-styled/v4';

import { LinkContainer } from 'react-router-bootstrap';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { colors } from 'libs/theme';

class SessionRow extends Component {
  render() {
    return (
      <Wrapper className="rounded mb-3">
        <Row data-cy="session-row" className="text-center p-3">
          <Col>
            <LinkContainer
              to={`/home/sessions/id/${this.props.sessionData.id}/`}
              data-cy="session-link"
            >
              <A className="font-weight-normal">
                {this.props.sessionData.id}
              </A>
            </LinkContainer>
          </Col>
          <Col>{"By: " + this.props.sessionData.fullname}</Col>
          <Col data-cy="session-date" className="d-none d-lg-table-cell">
            {"Created: " + DateTime.fromMillis(this.props.sessionData.createdAt).toRelative()}
          </Col>
          {this.props.buttonData !== undefined &&
            <Col>
              <FontAwesomeIcon
                icon={this.props.buttonData.icon}
                color={colors.primary}
                onClick={() => this.props.buttonData.onClick(this.props.sessionData.id)}
                data-cy="session-button"
              >
                {this.props.buttonData.body}
              </FontAwesomeIcon>
            </Col>
          }
        </Row>
      </Wrapper>
    );
  }
}

export default SessionRow;