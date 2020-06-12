import React from 'react';
import { shallow, mount, } from 'enzyme';
import Footer from 'components/Footer';

import validator from 'validator';

it('has non-empty content', () => {
  const message = shallow(<Footer />);

  expect(message.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const message = mount(<Footer />);

  expect(message.find('a').exists()).toBeTruthy();
});

it('has uppercase text', () => {
  const message = mount(<Footer />);

  expect(validator.isUppercase(message.find('a').text())).toBeTruthy();
});

it('has valid link', () => {
  const message = mount(<Footer />);

  expect(validator.isURL(message.find('a').props().href)).toBeTruthy();
});