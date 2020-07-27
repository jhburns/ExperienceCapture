import React, { Component } from 'react';

import { Wrapper } from 'pages/NotFound/style';
import { Link } from 'react-router-dom';

import { Container, Row, Col } from '@bootstrap-styled/v4';

import Menu from 'components/Menu';

import Footer from "components/Footer";

class NotFound extends Component {
  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="mt-5 mb-3">
            <Col>
              <h1>Page Not Found</h1>
              <h3>Error: 404</h3>
            </Col>
          </Row>
          <Row className="justify-content-center">
            <Col xs={6} md={5} xl={3}>
              <Link
                to="/"
                className="btn btn-outline-dark btn-block"
                data-cy="sign-in-link"
              >
                Try Signing In
              </Link>
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default NotFound;
