import React from 'react';
import { shallow, mount } from 'enzyme';
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

it('has valid links', () => {
  const wrapper = mount(<Footer />);

  wrapper.find('a').forEach((node) => {
    expect(validator.isURL(node.props().href, { require_host: false })).toBeTruthy();
  });
});