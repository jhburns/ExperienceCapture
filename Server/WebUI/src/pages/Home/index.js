import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Home/style';

class HomePage extends Component {
  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="pr-0 justify-content-center">
            <Col lg={10} className="pr-0 pl-xl-0">
              <SessionTable
                sessionsQuery={"createdWithin=1800&isOpen=true"} /* 30 minutes */
                isRenderingDate={false}
                emptyMessage="No active sessions right now."
                title="Open Sessions"
              />
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default HomePage;
