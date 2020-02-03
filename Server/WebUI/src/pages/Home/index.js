import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Home/style';

class HomePage extends Component {
  render() {
    return (
      <Wrapper>
        <Container>
          <Menu />
        </Container>
        <Row>
          <Col>
            <SessionTable
              sessionsQuery={"createdWithin=1800&isOpen=true"} /* 30 minutes */
              isRenderingDate={false}
              emptyMessage="No active sessions right now."
            />
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default HomePage;
