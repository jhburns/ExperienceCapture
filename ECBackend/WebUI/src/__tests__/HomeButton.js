import React from 'react';
import { shallow, mount, } from 'enzyme';
import HomeButton from 'components/HomeButton';

import { StaticRouter as Router } from "react-router-dom";
import validator from 'validator';

it('has non-empty content', () => {
  const button = shallow(<Router><HomeButton /></Router>);

  expect(button.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const button = mount(<Router><HomeButton /></Router>);

  expect(button.find('a').exists()).toBeTruthy();
});

it('has a valid link', () => {
  const button = mount(<Router><HomeButton /></Router>);

  expect(validator.isURL(button.find('a').props().href, { require_host: false, })).toBeTruthy();
});