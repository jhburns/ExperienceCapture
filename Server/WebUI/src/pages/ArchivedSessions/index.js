import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Link } from 'react-router-dom';

import { Wrapper } from 'pages/ArchivedSessions/style';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import Footer from "components/Footer";

class ArchivedSessionsPage extends Component {
  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="justify-content-center">
            <Col lg={10} className="pr-0">
              <SessionTable
                sessionsQuery={"isOngoing=false"}
                buttonData={{
                  isAdd: false,
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
          <Row className="m-0 mt-3 mb-5">
            <Col className="text-center">
              <Link to="/home/sessions" className="btn btn-outline-dark">
                Back
              </Link>
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default ArchivedSessionsPage;
