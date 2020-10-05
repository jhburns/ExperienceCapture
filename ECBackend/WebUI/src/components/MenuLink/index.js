import React, { Component } from 'react';

import { NavLink, NavItem } from "@bootstrap-styled/v4";

import { Wrapper, NavLinkOverride, Underline } from 'components/MenuLink/style';

import { LinkContainer } from 'react-router-bootstrap';

class MenuLink extends Component {
  render() {
    return (
      <Wrapper>
        <NavItem>
          <Underline>
            <LinkContainer to={this.props.to} data-cy={`menu-link-${this.props.suffix}`} >
              <NavLink
                as={NavLinkOverride}
                className="text-decoration-none ml-4 font-weight-medium mt-3 mb-3"
              >
                {this.props.linkText}
              </NavLink>
            </LinkContainer>
          </Underline>
        </NavItem>
      </Wrapper>
    );
  }
}

export default MenuLink;