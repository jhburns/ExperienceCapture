import React, { Component } from 'react';

import { Wrapper, GiantText } from 'pages/NotFound/style';

import { Container, Row, Col, P, A } from '@bootstrap-styled/v4';

import Header from 'components/Header';

import Footer from "components/Footer";

import { LinkContainer } from 'react-router-bootstrap';

class NotFound extends Component {
  render() {
    return (
      <Wrapper>
        <Container className="p-0">
          <Header/>
          <Row className="mt-2 mb-5 pb-5">
            <Col className="text-center">
              <GiantText className="font-weight-bold m-0">404</GiantText>
            </Col>
          </Row>
          <Row>
            <Col className="text-center">
              <P>Page not found,&nbsp;
                <LinkContainer to="/">
                  <A to="/" data-cy="sign-in-link">
                      leave and sign in.
                  </A>
                </LinkContainer>
              </P>
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    );
  }
}

export default NotFound;
