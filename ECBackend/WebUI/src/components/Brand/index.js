import React, { Component } from 'react';

import logo from 'img/logo.svg';

import { Image } from "components/Brand/style";

import { LinkContainer } from 'react-router-bootstrap';

import { A } from '@bootstrap-styled/v4';

class Brand extends Component {
  render() {
    return (
      <LinkContainer
        to={this.props.to}
        className={this.props.className}
        data-cy="menu-brand"
      >
        <A className="font-weight-bold text-decoration-none pr-4">
          <Image
            className="d-inline-block align-top mr-5"
            src={logo}
            alt="logo"
          />
          Experience Capture
        </A>
      </LinkContainer>
    );
  }
}

export default Brand;