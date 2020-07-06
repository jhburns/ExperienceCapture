import React from 'react';
import { shallow, render, } from 'enzyme';
import App from '../App';

it('renders without crashing', () => {
  render(<App />);
});

it('exists and has class name App', () => {
  const wrapper = shallow(<App />);

  expect(wrapper.find('.App').exists()).toBeTruthy();
});