import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';
import Tutorial from 'components/Tutorial';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Home/style';

import Footer from "components/Footer";

class HomePage extends Component {
  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="mr-0 ml-0 pr-0 justify-content-center">
            <Col lg={10} className="pr-0 pl-0">
              <SessionTable
                sessionsQuery={"isOngoing=true"}
                isRenderingDate={false}
                emptyMessage="No active sessions right now."
                title="Ongoing Sessions"
              />
            </Col>
          </Row>
          <Row className="m-0 justify-content-center">
            <Col lg={10} className="p-0">
              <Tutorial />
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default HomePage;