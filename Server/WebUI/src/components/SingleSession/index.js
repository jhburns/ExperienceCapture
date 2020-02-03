import React, { Component } from 'react';

import moment from 'moment';

import { Link } from 'react-router-dom';

import { Row, Col } from '@bootstrap-styled/v4';

import { Wrapper } from 'components/SingleSession/style';


class SingleSession extends Component {
  render() {
    const isExportDisabled = this.props.sessionData.isPending || this.props.sessionData.isExported;

    return (
      <Wrapper>
        <Row>
          <Col>
            <h2>Session: {this.props.sessionData.id}</h2>
            <h5>Captured By: {this.props.sessionData.fullname}</h5>
            <h5 className="mb-4">
              {moment(this.props.sessionData.createdAt).format("MMM Do YY hh:mm a")}
            </h5>
            <button
              onClick={this.props.onExport}
              disabled={isExportDisabled}
              className="btn btn-dark mr-2"
            >
              Export
            </button>
            <Link
              to={`/api/v1/sessions/${this.props.sessionData.id}/export/`}
              target="_blank"
              download
              className="btn btn-outline-dark"
            >
              Download
        </Link>
          </Col>
        </Row>
      </Wrapper>
    )
  }
}

export default SingleSession;