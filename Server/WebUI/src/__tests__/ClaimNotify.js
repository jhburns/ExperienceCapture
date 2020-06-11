import React from 'react';
import { shallow, mount, } from 'enzyme';
import ClaimNotify from 'components/ClaimNotify';

import { StaticRouter as Router } from "react-router-dom";

it('has non-empty content', () => {
  const message = shallow(<Router><ClaimNotify /></Router>);

  expect(message.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const message = mount(<Router><ClaimNotify /></Router>);

  expect(message.find('span').exists()).toBeTruthy();
  expect(message.text().includes('✔️')).toBeTruthy();
});