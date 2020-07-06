import React from 'react';
import { shallow, mount, } from 'enzyme';
import SignOutButton from 'components/SignOutButton';

it('has non-empty content', () => {
  const wrapper = shallow(<SignOutButton />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const wrapper = mount(<SignOutButton />);

  expect(wrapper.find('button').exists()).toBeTruthy();
});

it('callback is called on change', () => {
  const callback = jest.fn();
  const wrapper = mount(<SignOutButton onClickCallback={callback} />);

  wrapper.find('button').simulate('click');

  expect(callback).toHaveBeenCalledTimes(1);
});