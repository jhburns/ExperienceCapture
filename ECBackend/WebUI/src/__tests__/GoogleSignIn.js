import React from 'react';
import { shallow, mount, } from 'enzyme';
import GoogleSignIn from 'components/GoogleSignIn';

it('has non-empty content', () => {
  const wrapper = shallow(<GoogleSignIn />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('displays the correct message when the user can\'t sign in', () => {
  const wrapper = mount(<GoogleSignIn />);

  wrapper.setState({ isUnableToSignIn: true });

  expect(wrapper.text().includes('Sorry, there was an issue signing in.')).toBeTruthy();
});

it('displays the correct message when the user needs to sign in', () => {
  const wrapper = mount(<GoogleSignIn />);

  expect(wrapper.text().includes('Please Sign In.')).toBeTruthy();
});