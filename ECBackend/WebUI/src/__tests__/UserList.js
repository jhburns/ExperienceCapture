import React from 'react';
import { shallow, mount, } from 'enzyme';
import UserList from 'components/UserList';

it('has non-empty content', () => {
  const users = shallow(<UserList />);

  expect(users.find('h3').text().length).toBeGreaterThan(0);
});


it('lacks rows when users is empty', () => {
  const users = mount(<UserList />);

  expect(users.find('p')).toHaveLength(0);
  expect(users.find('button')).toHaveLength(0);
});


it('has row when there are some users', () => {
  const users = mount(<UserList />);

  const sampleUser = {
    fullname: 'Smitty Jensens',
    id: '1234'
  };
  users.setState({ users: [sampleUser, sampleUser] });

  expect(users.find('p').length).toBeGreaterThan(0);
  expect(users.find('button').length).toBeGreaterThan(0);
});

it('has clickable buttons', () => {
  const users = mount(<UserList />);

  const sampleUser = {
    fullname: 'Smitty Jensens',
    id: '1234'
  };
  users.setState({ users: [sampleUser] });

  users.find('button').simulate('click');
});