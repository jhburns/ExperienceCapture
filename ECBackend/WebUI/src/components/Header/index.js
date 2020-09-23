import React, { Component } from 'react';

import { Row, Col } from '@bootstrap-styled/v4';

import { Wrapper, Logo, Title } from 'components/Header/style.js';

import logo from 'img/logo.svg';

class Header extends Component {
  render() {
    return (
      <Wrapper>
        <Row className="pt-4 mb-4 pb-5" noGutters={true}>
          <Col className="d-flex align-items-center pr-0" xs="5" sm="4" lg="2">
            <Logo src={logo} alt="logo" />
          </Col>
          <Col className="d-flex align-items-center pl-0">
            <Title className="mb-0 font-weight-bold">
              Experience Capture
            </Title>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default Header;