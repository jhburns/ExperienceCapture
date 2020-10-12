import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Container, Row, Col } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Sessions/style';

import Footer from "components/Footer";

import { faArchive } from '@fortawesome/free-solid-svg-icons';

class SessionsPage extends Component {
  render() {
    return (
      <Wrapper>
        <Menu />
        <Container className="pb-5">
          <Row className="justify-content-center">
            <Col lg={10} className="pr-0">
              <SessionTable
                queryOptions={{ isOngoing: false, lacksTags: "archived" }}
                buttonData={{
                  isAdd: true,
                  icon: faArchive,
                }}
                emptyMessage="There are no complete sessions."
                title="Completed Sessions"
                link={{ name: "Archived >", path: "/home/sessions/archived"}}
              />
            </Col>
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default SessionsPage;