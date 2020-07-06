import React from 'react';
import { shallow, mount, } from 'enzyme';
import GetSignUpLink from 'components/GetSignUpLink';

it('has non-empty content', () => {
  const button = shallow(<GetSignUpLink />);

  expect(button.text().length).toBeGreaterThan(0);
});

it('is empty at on creation', () => {
  const button = mount(<GetSignUpLink />);

  expect(button.state().link).toBe("");
});

it('is lacks text on creation', () => {
  const button = mount(<GetSignUpLink />);

  expect(button.find('p').exists()).toBe(false);
});

it('presents link when created', () => {
  const button = mount(<GetSignUpLink />);

  button.setState({ link: "something"});

  expect(button.find('p').exists()).toBeTruthy();
  expect(button.find('p').text()).toBe('something');
});