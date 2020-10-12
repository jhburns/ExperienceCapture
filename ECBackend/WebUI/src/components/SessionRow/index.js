import React, { Component } from 'react';

import { DateTime } from 'luxon';

import { Wrapper } from 'components/SessionRow/style';

import { A, Th, Td, Table, Tbody } from '@bootstrap-styled/v4';

import { LinkContainer } from 'react-router-bootstrap';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { colors } from 'libs/theme';

class SessionRow extends Component {
  render() {
    return (
      <Table>
        <Tbody>
          <Wrapper className="rounded">
            <Th scope="row" data-cy="session-row">
              <LinkContainer
                to={`/home/sessions/id/${this.props.sessionData.id}/`}
                data-cy="session-link"
              >
                <A className="font-weight-normal pl-3">
                  {this.props.sessionData.id}
                </A>
              </LinkContainer>
            </Th>
            <Td>{"By: " + this.props.sessionData.fullname}</Td>
            <Td data-cy="session-date" className="d-none d-lg-table-cell">
              {"Created: " + DateTime.fromMillis(this.props.sessionData.createdAt).toRelative()}
            </Td>
            {this.props.buttonData !== undefined &&
              <Td>
                <FontAwesomeIcon
                  icon={this.props.buttonData.icon}
                  color={colors.primary}
                  onClick={() => this.props.buttonData.onClick(this.props.sessionData.id)}
                  data-cy="session-button"
                >
                  {this.props.buttonData.body}
                </FontAwesomeIcon>
              </Td>
            }
          </Wrapper>
        </Tbody>
      </Table>
    );
  }
}

export default SessionRow;