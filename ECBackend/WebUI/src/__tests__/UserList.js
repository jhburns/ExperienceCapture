import React from 'react';
import { shallow, mount } from 'enzyme';
import UserList from 'components/UserList';

import { Button, P, H3 } from '@bootstrap-styled/v4';

it('has non-empty content', () => {
  const wrapper = shallow(<UserList />);

  expect(wrapper.find(H3).text().length).toBeGreaterThan(0);
});


it('lacks rows when users is empty', () => {
  const wrapper = mount(<UserList />);

  expect(wrapper.find(P)).toHaveLength(1);
  expect(wrapper.find(Button)).toHaveLength(0);
});


it('has row when there are some users', () => {
  const wrapper = mount(<UserList />);

  const sampleUser = {
    fullname: 'Smitty Jensens',
    id: '1234',
  };

  wrapper.setState({ users: [sampleUser, sampleUser] });

  expect(wrapper.find(P).length).toBeGreaterThan(0);
  expect(wrapper.find(Button).length).toBeGreaterThan(0);
});

it('has clickable buttons', () => {
  const wrapper = mount(<UserList />);

  const sampleUser = {
    fullname: 'Smitty Jensens',
    id: '1234',
  };

  wrapper.setState({ users: [sampleUser] });

  wrapper.find('button').simulate('click');
});