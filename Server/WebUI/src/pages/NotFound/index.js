import React, { Component } from 'react';

import { Wrapper } from 'pages/NotFound/style';
import { Link } from 'react-router-dom';

import { Container, Row, Col, } from '@bootstrap-styled/v4';

class NotFound extends Component {
  render() {
    return (
      <Wrapper>
        <Container>
          <Row className="mt-5 mb-3">
            <Col>
              <h1>Page Not Found</h1>
              <h3>Error: 404</h3>
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6}>
              <Link
                to="/"
                className="btn btn-outline-dark btn-block"
              >
                Try Login
              </Link>
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default NotFound;
