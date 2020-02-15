import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { deleteData } from 'libs/fetchExtra';

import { Link } from 'react-router-dom';

import { Wrapper } from 'pages/ArchivedSessions/style';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

class ArchivedSessionsPage extends Component {
  constructor(props) {
    super(props);

    this.archiveCallback = this.onArchive.bind(this);
  }

  async onArchive(id) {
    try {
      const dearchiveRequest = await deleteData(`/api/v1/sessions/${id}/tags/archived`);

      if (!dearchiveRequest.ok) {
        throw Error(dearchiveRequest.status);
      }
    } catch (err) {
      console.error(err);
    }
  }

  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="pr-0 justify-content-center">
            <Col lg={10} className="pr-0 pl-xl-0">
              <SessionTable
                sessionsQuery={"isOngoing=false"}
                buttonData={{
                  onClick: this.archiveCallback,
                  body: "Unarchive",
                  header: ""
                }}
                hasTag={"archived"}
                isRenderingDate={true}
                emptyMessage="No archived sessions."
                title="Archived Sessions"
              />
            </Col>
          </Row>
          <Row className="mt-3 mb-5">
            <Col className="text-center">
              <Link to="/home/sessions" className="btn btn-outline-dark">
                Back
              </Link>
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default ArchivedSessionsPage;
