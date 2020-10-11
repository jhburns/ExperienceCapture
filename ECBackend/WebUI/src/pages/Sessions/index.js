import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Link } from 'react-router-dom';

import { Container, Row, Col } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Sessions/style';

import Footer from "components/Footer";

class SessionsPage extends Component {
  render() {
    return (
      <Wrapper>
        <Menu />
        <Container className="pb-5">
          <Row className="mr-0 pr-0 justify-content-center">
            <Col lg={10} className="pr-0">
              <SessionTable
                queryOptions={{ isOngoing: false, lacksTags: "archived" }}
                buttonData={{
                  isAdd: true,
                  body: "Archive",
                  header: "",
                }}
                isRenderingDate={true}
                emptyMessage="There are no complete sessions."
                title="Completed Sessions"
              />
            </Col>
          </Row>
          <Row className="m-0 mt-3 mb-5">
            <Col className="text-center">
              <Link
                to="/home/sessions/archived"
                className="btn btn-outline-dark"
                data-cy="archive-link"
              >
                Archived
              </Link>
            </Col>
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default SessionsPage;