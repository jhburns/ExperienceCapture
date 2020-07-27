import React from 'react';
import { shallow, mount } from 'enzyme';
import GetSignUpLink from 'components/GetSignUpLink';

it('has non-empty content', () => {
  const wrapper = shallow(<GetSignUpLink />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('is empty at on creation', () => {
  const wrapper = mount(<GetSignUpLink />);

  expect(wrapper.state().link).toBe("");
});

it('is lacks text on creation', () => {
  const wrapper = mount(<GetSignUpLink />);

  expect(wrapper.find('p').exists()).toBe(false);
});

it('presents link when created', () => {
  const wrapper = mount(<GetSignUpLink />);

  wrapper.setState({ link: "something"});

  expect(wrapper.find('p').exists()).toBeTruthy();
  expect(wrapper.find('p').text()).toBe('something');
});