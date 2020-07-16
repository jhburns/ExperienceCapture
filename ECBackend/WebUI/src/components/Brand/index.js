import React, { Component } from 'react';
import { Link } from "react-router-dom";

import logo from 'logo.svg';

import { Image } from "components/Brand/style";

class Brand extends Component {
  render() {
    return (
      <Link to={this.props.to} className={this.props.className} data-cy="menu-brand">
        <Image
          className="d-inline-block align-top mr-3"
          src={logo}
          alt="logo"
        />
        Experience Capture
      </Link>
    );
  }
}

export default Brand;