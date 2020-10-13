import React from 'react';
import { shallow, mount } from 'enzyme';
import SessionRow from 'components/SessionRow';

import { StaticRouter as Router } from "react-router-dom";
import validator from 'validator';

import { faTrashRestore } from '@fortawesome/free-solid-svg-icons';

it('has non-empty content', () => {
  const wrapper = shallow(<SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", createdAt: 1591840213871}} />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('has valid link', () => {
  const jsx =
    <Router>
      <SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", createdAt: 1591840213871 }} />
    </Router>;

  const wrapper = mount(jsx);

  expect(validator.isURL(wrapper.find('a').props().href, { require_host: false })).toBeTruthy();
});

it('has valid link', () => {
  const jsx =
    <Router>
      <SessionRow sessionData={{ id: "test/link", fullname: "Smitty Jensens", createdAt: 1591840213871 }} />
    </Router>;

  const wrapper = mount(jsx);

  expect(validator.isURL(wrapper.find('a').props().href, { require_host: false })).toBeTruthy();
});

it('does not have icon when undefined', () => {
  const jsx =
    <Router>
      <SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", createdAt: 1591840213871 }} />
    </Router>;

  const wrapper = mount(jsx);

  expect(wrapper.find('path')).toHaveLength(0);
});

it('does have icon when defined', () => {
  const jsx =
    <Router>
      <SessionRow
        sessionData={{ id: "EXEX", fullname: "Smitty Jensens", createdAt: 1591840213871 }}
        buttonData={{ icon: faTrashRestore }}
      />
    </Router>;

  const wrapper = mount(jsx);

  expect(wrapper.find('path')).toHaveLength(1);
});

it('calls on click', () => {
  const callback = jest.fn();

  const jsx =
    <Router>
      <SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", createdAt: 1591840213871 }}
        buttonData={{ onClick: callback, icon: faTrashRestore }}
      />
    </Router>;

  const wrapper = mount(jsx);

  wrapper.find('path').simulate('click');

  expect(callback).toHaveBeenCalledTimes(1);
});