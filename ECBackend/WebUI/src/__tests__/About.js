import React from 'react';
import { shallow, mount, } from 'enzyme';
import About from 'components/About';

it('has non-empty content', () => {
  const wrapper = shallow(<About />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const wrapper = mount(<About />);

  expect(wrapper.find('a').exists()).toBeTruthy();
});

it('has an initial state of closed', () => {
  const wrapper = mount(<About />);

  expect(wrapper.state().isOpen).toBe(false);
});