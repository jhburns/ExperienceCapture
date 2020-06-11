import React from 'react';
import { shallow, mount, } from 'enzyme';
import MenuLink from 'components/MenuLink';

import { StaticRouter as Router } from "react-router-dom";

it('has non-empty content', () => {
  const menu = shallow(<MenuLink linkText="text" locationPath="location" to="path" />);

  expect(menu.text().length).toBeGreaterThan(0);
});

it('uses given text', () => {
  const menu = mount(<Router><MenuLink linkText="text" locationPath="location" to="path" /></Router>);

  expect(menu.text().includes('text')).toBeTruthy();
});

it('has no body if text is empty', () => {
  const menu = mount(<Router><MenuLink linkText="" locationPath="location" to="path" /></Router>);

  expect(menu.text().length).toBe(0);
});

it('is active when location and to are the same', () => {
  const menu = mount(<Router><MenuLink linkText="" locationPath="/location" to="/location" /></Router>);

  expect(menu.find('a.nav-link').hasClass('active')).toBeTruthy();
});

it('is active when location and to are similar', () => {
  const menu = mount(<Router><MenuLink linkText="" locationPath="/location/longer" to="/location" /></Router>);

  expect(menu.find('a.nav-link').hasClass('active')).toBeTruthy();
});

it('is not active when location and to are different', () => {
  const menu = mount(<Router><MenuLink linkText="" locationPath="/location" to="/location2" /></Router>);

  expect(menu.find('a.nav-link').hasClass('active')).toBe(false);
});

it('is not active when location and to are dissimilar', () => {
  const menu = mount(<Router><MenuLink linkText="" locationPath="/location" to="/location/longer" /></Router>);

  expect(menu.find('a.nav-link').hasClass('active')).toBe(false);
});