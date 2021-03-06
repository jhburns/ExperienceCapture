import React, { Component } from 'react';

import { Wrapper, Background, Hamburger, Toggle } from 'components/Menu/style';

import {
  Nav,
  Navbar,
  Container,
  Collapse,
  NavbarBrand,
  Row,
  Col,
} from "@bootstrap-styled/v4";

import Brand from 'components/Brand';

import { withRouter } from "react-router";

import MenuLink from "components/MenuLink";

import hamburgerMenu from 'img/hamburger_menu.svg';
import hamburgerMenuActive from 'img/hamburger_menu_active.svg';

class Menu extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isOpen: false,
    };
  }

  render() {
    const { location } = this.props;

    const hamburgers = {
      inactive: hamburgerMenu,
      active: hamburgerMenuActive,
    };
    const imageOption = this.state.isOpen ? "active" : "inactive";


    return (
      <Wrapper>
        <Container>
          <Navbar light toggleable="md" className="mt-3 mb-3 mb-xl-4">
            <Row className="d-flex flex-wrap align-items-center">
              {/* The # prevents the brand from getting active on accident */}
              <Col xs={8} lg="auto">
                <NavbarBrand className="pl-3 pl-lg-0" tag={Brand} to="/home/start#" />
              </Col>
              <Col xs={4} lg="auto" className="text-center">
                <Toggle
                  onClick={() => this.setState({ isOpen: !this.state.isOpen })}
                  className="navbar-toggler"
                  data-cy="menu-hamburger"
                  aria-label="Navbar toggle"
                  isOpen={this.state.isOpen}
                >
                  <Hamburger alt="Hamburger menu" src={hamburgers[imageOption]} isOpen={this.state.isOpen}></Hamburger>
                </Toggle>
              </Col>
              <Col className="p-0">
                <Collapse navbar isOpen={this.state.isOpen} data-cy="menu-collapse">
                  <Nav navbar className="mr-auto d-none d-lg-flex">
                    <MenuLink locationPath={location.pathname} to="/home/start" linkText="Home" suffix="expand" />
                    <MenuLink locationPath={location.pathname} to="/home/sessions" linkText="Sessions" suffix="expand" />
                    <MenuLink locationPath={location.pathname} to="/home/settings" linkText="Settings" suffix="expand" />
                  </Nav>
                  <Nav navbar className="d-lg-none mt-4">
                    <Background className="rounded">
                      <MenuLink locationPath={location.pathname} to="/home/start" linkText="Home" suffix="collapse"/>
                      <MenuLink locationPath={location.pathname} to="/home/sessions" linkText="Sessions" suffix="collapse"/>
                      <MenuLink locationPath={location.pathname} to="/home/settings" linkText="Settings" suffix="collapse" />
                    </Background>
                  </Nav>
                </Collapse>
              </Col>
            </Row>
          </Navbar>
        </Container>
      </Wrapper>
    );
  }
}

const MenuWithRouter = withRouter(Menu);

export default MenuWithRouter;