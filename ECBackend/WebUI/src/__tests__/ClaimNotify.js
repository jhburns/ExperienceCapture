import React from 'react';
import { shallow, mount } from 'enzyme';
import ClaimNotify from 'components/ClaimNotify';

import { StaticRouter as Router } from "react-router-dom";

it('has non-empty content', () => {
  const wrapper = shallow(<Router><ClaimNotify /></Router>);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const wrapper = mount(<Router><ClaimNotify /></Router>);

  expect(wrapper.find('span').exists()).toBeTruthy();
  expect(wrapper.text().includes('✔️')).toBeTruthy();
});