import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Wrapper } from 'pages/ArchivedSessions/style';

import { Container, Row, Col } from '@bootstrap-styled/v4';

import Footer from "components/Footer";

import { faTrashRestore } from '@fortawesome/free-solid-svg-icons';

class ArchivedSessionsPage extends Component {
  render() {
    return (
      <Wrapper>
        <Menu />
        <Container className="pb-5">
          <Row className="justify-content-center">
            <Col lg={10} className="pr-0">
              <SessionTable
                queryOptions={{ isOngoing: false, hasTags: "archived" }}
                buttonData={{
                  isAdd: false,
                  icon: faTrashRestore,
                }}
                emptyMessage="There are no archived sessions."
                title="Archived Sessions"
                link={{ name: "Completed >", path: "/home/sessions/#" }}
              />
            </Col>
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default ArchivedSessionsPage;
