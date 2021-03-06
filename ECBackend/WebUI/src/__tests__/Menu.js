import React from 'react';
import { shallow, mount } from 'enzyme';
import Menu from 'components/Menu';

import { StaticRouter as Router } from "react-router-dom";
import validator from 'validator';

it('has non-empty content', () => {
  const wrapper = shallow(<Router><Menu /></Router>);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const wrapper = mount(<Router><Menu /></Router>);

  expect(wrapper.find('nav').exists()).toBeTruthy();
});

it('has a valid link for brand', () => {
  const wrapper = mount(<Router><Menu /></Router>);

  expect(validator.isURL(wrapper.find('a.navbar-brand').props().href, { require_host: false })).toBeTruthy();
});

it('has valid paths', () => {
  const wrapper = mount(<Router><Menu /></Router>);

  wrapper.find('a.nav-link').forEach((node) => {
    expect(validator.isURL(node.props().href, { require_host: false })).toBeTruthy();
  });
});

it('has navlinks with contents', () => {
  const wrapper = mount(<Router><Menu /></Router>);

  wrapper.find('a.nav-link').forEach((node) => {
    expect(node.text().length).toBeGreaterThan(0);
  });
});