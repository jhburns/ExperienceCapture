import React, { Component } from 'react';

import { NavLink, NavItem } from "@bootstrap-styled/v4";

import { Wrapper, NavLinkOverride } from 'components/MenuLink/style';

import { LinkContainer } from 'react-router-bootstrap';

class MenuLink extends Component {
  render() {
    return (
      <Wrapper>
        <NavItem>
          <LinkContainer to={this.props.to} data-cy="menu-link" >
            <NavLink
              as={NavLinkOverride}
              className="text-decoration-none ml-4 font-weight-medium"
            >
              {this.props.linkText}
            </NavLink>
          </LinkContainer>
        </NavItem>
      </Wrapper>
    );
  }
}

export default MenuLink;