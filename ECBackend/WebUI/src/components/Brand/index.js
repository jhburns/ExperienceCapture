import React, { Component } from 'react';

import logo from 'img/logo.svg';

import { Image, Title } from "components/Brand/style";

import { LinkContainer } from 'react-router-bootstrap';

class Brand extends Component {
  render() {
    return (
      <LinkContainer
        to={this.props.to}
        className={this.props.className}
        data-cy="menu-brand"
      >
        <Title className="font-weight-bold text-decoration-none pr-4">
          <Image
            className="d-none d-lg-inline-block align-top mr-5"
            src={logo}
            alt="logo"
          />
          Experience <br className="d-lg-none"/> Capture
        </Title>
      </LinkContainer>
    );
  }
}

export default Brand;