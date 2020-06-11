import React from 'react';
import { shallow, mount, } from 'enzyme';
import Menu from 'components/Menu';

import { StaticRouter as Router } from "react-router-dom";
import validator from 'validator';

it('has non-empty content', () => {
  const menu = shallow(<Router><Menu /></Router>);

  expect(menu.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const menu = mount(<Router><Menu /></Router>);

  expect(menu.find('nav').exists()).toBeTruthy();
});

it('has a valid link for brand', () => {
  const menu = mount(<Router><Menu /></Router>);

  expect(validator.isURL(menu.find('a.navbar-brand').props().href, { require_host: false, })).toBeTruthy();
});

it('has valid paths', () => {
  const menu = mount(<Router><Menu /></Router>);

  menu.find('a.nav-link').forEach((node) => {
    expect(validator.isURL(node.props().href, { require_host: false, })).toBeTruthy();
  });
});

it('has navlinks with contents', () => {
  const menu = mount(<Router><Menu /></Router>);

  menu.find('a.nav-link').forEach((node) => {
    expect(node.text().length).toBeGreaterThan(0);
  });
});