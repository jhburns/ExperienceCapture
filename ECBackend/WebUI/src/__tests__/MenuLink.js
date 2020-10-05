import React from 'react';
import { shallow, mount } from 'enzyme';
import MenuLink from 'components/MenuLink';

import { StaticRouter as Router } from "react-router-dom";

it('has non-empty content', () => {
  const wrapper = shallow(<MenuLink linkText="text" locationPath="location" to="path" />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('uses given text', () => {
  const wrapper = mount(<Router><MenuLink linkText="text" locationPath="location" to="path" /></Router>);

  expect(wrapper.text().includes('text')).toBeTruthy();
});

it('has no body if text is empty', () => {
  const wrapper = mount(<Router><MenuLink linkText="" locationPath="location" to="path" /></Router>);

  expect(wrapper.text().length).toBe(0);
});