import React from 'react';
import { shallow, mount, } from 'enzyme';
import About from 'components/About';

it('has non-empty content', () => {
  const message = shallow(<About />);

  expect(message.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const message = mount(<About />);

  expect(message.find('a').exists()).toBeTruthy();
});

it('has an initial state of closed', () => {
  const message = mount(<About />);

  expect(message.state().isOpen).toBe(false);
});