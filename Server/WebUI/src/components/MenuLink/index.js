import React, { Component } from 'react';

import { Link } from 'react-router-dom';
import { NavLink, NavItem, } from "@bootstrap-styled/v4";

class MenuLink extends Component {
  render() {
    const sameLengthPath = this.props.locationPath.substring(0, this.props.to.length);
    const isActive = this.props.to == sameLengthPath;

    console.log(sameLengthPath);

    return (
      <NavItem>
        <NavLink active={isActive} tag={Link} to={this.props.to}>
          {this.props.linkText}
        </NavLink>
      </NavItem>
    )
  }
}

export default MenuLink;