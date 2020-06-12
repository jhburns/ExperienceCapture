import React from 'react';
import { shallow, mount, } from 'enzyme';
import SignOutButton from 'components/SignOutButton';

it('has non-empty content', () => {
  const button = shallow(<SignOutButton />);

  expect(button.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const button = mount(<SignOutButton />);

  expect(button.find('button').exists()).toBeTruthy();
});

it('callback is called on change', () => {
  const callback = jest.fn();
  const button = mount(<SignOutButton onClickCallback={callback} />);

  button.find('button').simulate('click');

  expect(callback).toHaveBeenCalledTimes(1);
});