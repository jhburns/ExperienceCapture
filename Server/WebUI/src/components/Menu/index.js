import React, { Component } from 'react';

import { Link }from 'react-router-dom';

import { Wrapper } from 'components/Menu/style';

import {
  Nav,
  NavItem,
  NavLink,
  Navbar,
  Container,
  Collapse,
  NavbarBrand,
  NavbarToggler,
} from "@bootstrap-styled/v4";

import Brand from 'components/Brand';

class Menu extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isOpen: false,
    }
  }

  render() {
    return (
      <Wrapper>
        <Navbar color="faded" light toggleable="lg" className="mt-3 mb-3 mb-xl-4">
          <Container>
            <NavbarBrand tag={Brand} to="/home" className="mr-3" />
            <NavbarToggler
              onClick={() => this.setState({ isOpen: !this.state.isOpen })}
              className="ml-auto"
            />
            <Collapse navbar isOpen={this.state.isOpen}>
              <Nav navbar className="mr-auto">
                <NavItem>
                  <NavLink active tag={Link} to="/home">
                    Home
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink active tag={Link} to="/home/sessions">
                    Sessions
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink active tag={Link} to="/home/settings">
                    Settings
                  </NavLink>
                </NavItem>
              </Nav>
            </Collapse>
          </Container>
        </Navbar>
      </Wrapper>
    )
  }
}
export default Menu;