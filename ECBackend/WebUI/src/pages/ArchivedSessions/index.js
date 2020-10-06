import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Link } from 'react-router-dom';

import { Wrapper } from 'pages/ArchivedSessions/style';

import { Container, Row, Col } from '@bootstrap-styled/v4';

import Footer from "components/Footer";

class ArchivedSessionsPage extends Component {
  render() {
    return (
      <Wrapper>
        <Menu />
        <Container nogutters className="pb-5">
          <Row className="justify-content-center">
            <Col lg={10} className="pr-0">
              <SessionTable
                queryOptions={{ isOngoing: false, hasTags: "archived" }}
                buttonData={{
                  isAdd: false,
                  body: "Unarchive",
                  header: "",
                }}
                isRenderingDate={true}
                emptyMessage="No archived sessions."
                title="Archived Sessions"
              />
            </Col>
          </Row>
          <Row className="m-0 mt-3 mb-5">
            <Col className="text-center">
              <Link
                to="/home/sessions"
                className="btn btn-outline-dark"
                data-cy="sessions-link"
              >
                Back
              </Link>
            </Col>
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default ArchivedSessionsPage;
