import React from 'react';
import { shallow, render, } from 'enzyme';
import App from '../App';

it('renders without crashing', () => {
  render(<App />);
});

it('exists and has class name App', () => {
  const app = shallow(<App />);

  expect(app.find('.App').exists()).toBeTruthy();
});