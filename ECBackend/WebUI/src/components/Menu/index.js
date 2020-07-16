import React, { Component } from 'react';

import { Wrapper } from 'components/Menu/style';

import {
  Nav,
  Navbar,
  Container,
  Collapse,
  NavbarBrand,
  NavbarToggler,
} from "@bootstrap-styled/v4";

import Brand from 'components/Brand';

import { withRouter } from "react-router";

import MenuLink from "components/MenuLink";

class Menu extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isOpen: false,
    };
  }

  render() {
    const { location } = this.props;

    return (
      <Wrapper>
        <Navbar color="faded" light toggleable="lg" className="mt-3 mb-3 mb-xl-4">
          <Container>
            <NavbarBrand tag={Brand} to="/home/start" />
            <NavbarToggler
              onClick={() => this.setState({ isOpen: !this.state.isOpen })}
              className="ml-auto"
              data-cy="menu-hamburger"
              aria-label="Navbar toggle"
            >
            </NavbarToggler>
            <Collapse navbar isOpen={this.state.isOpen} data-cy="menu-collapse">
              <Nav navbar className="mr-auto">
                <MenuLink locationPath={location.pathname} to="/home/start" linkText="Home" />
                <MenuLink locationPath={location.pathname} to="/home/sessions" linkText="Sessions" />
                <MenuLink locationPath={location.pathname} to="/home/settings" linkText="Settings" />
              </Nav>
            </Collapse>
          </Container>
        </Navbar>
      </Wrapper>
    );
  }
}

const MenuWithRouter = withRouter(Menu);

export default MenuWithRouter;