import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';
import Tutorial from 'components/Tutorial';

import { Container, Row, Col } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Home/style';

import Footer from "components/Footer";

class HomePage extends Component {
  render() {
    return (
      <Wrapper>
        <Menu />
        <Container className="pb-5">
          <Row className="justify-content-center">
            <Col lg={10} className="pr-0">
              <SessionTable
                queryOptions={{ isOngoing: true }}
                emptyMessage="There are no ongoing sessions."
                title="Ongoing Sessions"
              />
            </Col>
          </Row>
          <Row className="m-0 justify-content-center">
            <Col lg={10} className="p-0">
              <Tutorial />
            </Col>
          </Row>
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default HomePage;