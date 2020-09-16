import React, { Component } from 'react';

import { DateTime } from 'luxon';

import { Link } from 'react-router-dom';

import { Row, Col } from '@bootstrap-styled/v4';

import { Wrapper } from 'components/SingleSession/style';

import About from "components/About";

class SingleSession extends Component {
  render() {
    const isExportDisabled = this.props.sessionData.exportState === "Pending"
      || this.props.sessionData.isOngoing;

    const isNotExported = !(this.props.sessionData.exportState === "Done");

    const about = "Sessions have to be exported, so that they can be converted to flat files. "
      + (isExportDisabled ? "Ongoing sessions can't be exported." : "");

    return (
      <Wrapper>
        <Row>
          <Col>
            <h2>Session: {this.props.sessionData.id}</h2>
            <h5>Captured By: {this.props.sessionData.fullname}</h5>
            <h5 className="mb-4">
              {DateTime.fromMillis(this.props.sessionData.createdAt).toLocaleString(DateTime.DATETIME_MED)}
            </h5>
            <h5 className="mb-4">Status: {
              !this.props.sessionData.isOpen ?
                "Completed"
                :
                this.props.sessionData.isOngoing ?
                  "Ongoing"
                  :
                  "Closed Unexpectedly"
            }
            </h5>
            {isNotExported ?
              <div>
                <button
                  onClick={this.props.onExport}
                  disabled={isExportDisabled}
                  className="btn btn-outline-dark"
                  data-cy="session-export"
                >
                  Export
                </button>
                <About message={about} />
              </div>
              :
              <Link
                to={`/api/v1/sessions/${this.props.sessionData.id}/export/`}
                target="_blank"
                rel="noopener noreferrer"
                download
                className="btn btn-outline-dark"
                data-cy="session-download"
              >
                Download
              </Link>
            }
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default SingleSession;