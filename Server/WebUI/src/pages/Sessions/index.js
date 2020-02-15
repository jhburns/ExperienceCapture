import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { postData } from 'libs/fetchExtra';

import { Link } from 'react-router-dom';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Sessions/style';

class SessionsPage extends Component {
  constructor(props) {
    super(props);

    this.archiveCallback = this.onArchive.bind(this);
  }

  async onArchive(id) {
    try {
      const archiveRequest = await postData(`/api/v1/sessions/${id}/tags/archived`);

      if (!archiveRequest.ok) {
        throw Error(archiveRequest.status);
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
                sessionsQuery={""}
                buttonData={{
                  onClick: this.archiveCallback,
                  body: "Archive",
                  header: ""
                }}
                lacksTag={"archived"}
                isRenderingDate={true}
                emptyMessage="No unarchived sessions."
                title="Closed Sessions"
              />
            </Col>
          </Row>
          <Row className="mt-3 mb-5">
            <Col className="text-center">
              <Link to="/home/archived" className="btn btn-outline-dark">
                Archived
              </Link>
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default SessionsPage;
