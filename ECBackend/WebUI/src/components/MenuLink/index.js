import React, { Component } from 'react';

import { Link } from 'react-router-dom';
import { NavLink, NavItem, } from "@bootstrap-styled/v4";

import { Wrapper } from 'components/MenuLink/style';

class MenuLink extends Component {
  render() {
    const sameLengthPath = this.props.locationPath.substring(0, this.props.to.length);
    const isActive = this.props.to === sameLengthPath;

    return (
      <Wrapper>
        <NavItem>
          <NavLink active={isActive} tag={Link} to={this.props.to} data-cy="menu-link">
            {this.props.linkText}
          </NavLink>
        </NavItem>
      </Wrapper>
    )
  }
}

export default MenuLink;