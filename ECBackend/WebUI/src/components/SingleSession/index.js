import React, { Component } from 'react';

import { DateTime } from 'luxon';

import { Row, Col, H2, P, Button, Strong } from '@bootstrap-styled/v4';

import { Wrapper, Background } from 'components/SingleSession/style';

import About from "components/About";

import { faCircleNotch, faFileDownload, faFileExport } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { colors } from 'libs/theme';

class SingleSession extends Component {
  render() {
    // Allow re-exporting on error, maybe it will fix thinks
    const isNotStartedExporting = this.props.sessionData.exportState === "NotStarted"
      || this.props.sessionData.exportState === "Error";

    const about = "Sessions have to be exported before they can be downloaded."
      + (this.props.sessionData.isOngoing ? " Ongoing sessions can't be exported/downloaded." : "");

    return (
      <Wrapper>
        <Row className="justify-content-center justify-content-lg-start">
          <Col className="d-none d-lg-block" lg={2}></Col>
          <Col xs={10} lg={6}>
            <Background className="rounded p-3">
              <H2 className="mb-3 text-center">{this.props.sessionData.id}</H2>
              <Row className="d-flex justify-content-center">
                <Col xs={12} lg="auto">
                  <P>Author: <Strong>{this.props.sessionData.fullname}</Strong></P>
                  <P>
                    Start Time:&nbsp;
                    <Strong>
                      {DateTime.fromISO(this.props.sessionData.createdAt).toLocaleString(DateTime.DATETIME_FULL)}
                    </Strong>
                  </P>
                  <P className="mb-4">Status: <Strong>{
                    !this.props.sessionData.isOpen ?
                      "Completed"
                      :
                      this.props.sessionData.isOngoing ?
                        "Ongoing"
                        :
                        "Closed Unexpectedly"
                  }</Strong>
                  </P>
                </Col>
              </Row>
              <Row>
                <Col className="d-flex justify-content-center">
                  {isNotStartedExporting ?
                    // If the session is ongoing, we can't export
                    <div>
                      <Button
                        onClick={this.props.onExport}
                        disabled={this.props.sessionData.isOngoing}
                        className="btn btn-outline-dark"
                        data-cy="session-export"
                      >
                        <FontAwesomeIcon icon={faFileExport} color={colors.background} />
                        &nbsp;&nbsp;Export
                      </Button>
                      <About message={about} />
                    </div>
                    // Else if in the middle of exporting, show icon
                    : this.props.sessionData.exportState === "Pending" ?
                      <>
                        <Button disabled>
                          <FontAwesomeIcon spin icon={faCircleNotch} color={colors.background} />
                          &nbsp;&nbsp;Exporting
                        </Button>
                        <About message={about} />
                      </>
                      :
                      // Else download  when done.
                      <Button
                        href={`/api/v1/sessions/${this.props.sessionData.id}/export/`}
                        target="_blank"
                        rel="noopener noreferrer"
                        download
                        data-cy="session-download"
                        className="text-decoration-none"
                      >
                        <FontAwesomeIcon icon={faFileDownload} color={colors.background} />
                        &nbsp;&nbsp;Download
                      </Button>
                  }
                </Col>
              </Row>
              {this.props.sessionData.exportState === "Error" &&
                <Row>
                  <Col>
                    <P>Exporting failed, try again.</P>
                  </Col>
                </Row>
              }
            </Background>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default SingleSession;