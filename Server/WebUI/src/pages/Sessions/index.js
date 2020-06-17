import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Link } from 'react-router-dom';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Sessions/style';

import Footer from "components/Footer";

class SessionsPage extends Component {
  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="mr-0 pr-0 justify-content-center">
            <Col lg={10} className="pr-0">
              <SessionTable
                sessionsQuery={"isOngoing=false&lacksTags=archived"}
                buttonData={{
                  isAdd: true,
                  body: "Archive",
                  header: ""
                }}
                isRenderingDate={true}
                emptyMessage="No unarchived sessions."
                title="Completed Sessions"
              />
            </Col>
          </Row>
          <Row className="m-0 mt-3 mb-5">
            <Col className="text-center">
              <Link to="/home/sessions/archived" className="btn btn-outline-dark">
                Archived
              </Link>
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default SessionsPage;
