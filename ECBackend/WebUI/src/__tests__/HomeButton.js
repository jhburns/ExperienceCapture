import React from 'react';
import { shallow, mount, } from 'enzyme';
import HomeButton from 'components/HomeButton';

import { StaticRouter as Router } from "react-router-dom";
import validator from 'validator';

it('has non-empty content', () => {
  const wrapper = shallow(<Router><HomeButton /></Router>);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const wrapper = mount(<Router><HomeButton /></Router>);

  expect(wrapper.find('a').exists()).toBeTruthy();
});

it('has a valid link', () => {
  const wrapper = mount(<Router><HomeButton /></Router>);

  expect(validator.isURL(wrapper.find('a').props().href, { require_host: false, })).toBeTruthy();
});