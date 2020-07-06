import React from 'react';
import { shallow, mount, } from 'enzyme';
import Footer from 'components/Footer';

import validator from 'validator';

it('has non-empty content', () => {
  const wrapper = shallow(<Footer />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const wrapper = mount(<Footer />);

  expect(wrapper.find('a').exists()).toBeTruthy();
});

it('has uppercase text', () => {
  const wrapper = mount(<Footer />);

  expect(validator.isUppercase(wrapper.find('a').text())).toBeTruthy();
});

it('has valid link', () => {
  const wrapper = mount(<Footer />);

  expect(validator.isURL(wrapper.find('a').props().href)).toBeTruthy();
});