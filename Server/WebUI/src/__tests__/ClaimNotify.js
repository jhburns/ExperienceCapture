import React from 'react';
import { shallow, mount, } from 'enzyme';
import ClaimNotify from 'components/ClaimNotify';

import { StaticRouter as Router, StaticRouter } from "react-router-dom";

it('has non-empty content', () => {
  const message = shallow(<StaticRouter><ClaimNotify /></StaticRouter>);

  expect(message.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const message = mount(<StaticRouter><ClaimNotify /></StaticRouter>);

  expect(message.find('span').exists()).toBeTruthy();
  expect(message.text().includes('✔️')).toBeTruthy();
});