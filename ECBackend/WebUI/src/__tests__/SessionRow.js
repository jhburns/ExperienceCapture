import React from 'react';
import { shallow, mount, } from 'enzyme';
import SessionRow from 'components/SessionRow';

import { StaticRouter as Router } from "react-router-dom";
import validator from 'validator';

it('has non-empty content', () => {
  const row = shallow(<SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", createdAt: 1591840213871}} />);

  expect(row.text().length).toBeGreaterThan(0);
});

it('has valid link', () => {
  const jsx =
    <Router>
      <table>
        <tbody>
          <SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", createdAt: 1591840213871 }} />
        </tbody>
      </table>
    </Router>;

  const row = mount(jsx);

  expect(validator.isURL(row.find('a').props().href, { require_host: false, })).toBeTruthy();
});

it('has valid link when complex', () => {
  const jsx =
    <Router>
      <table>
        <tbody>
          <SessionRow sessionData={{ id: "test/if-this/is-vaild?test=2w4r", fullname: "Smitty Jensens", createdAt: 1591840213871 }} />
        </tbody>
      </table>
    </Router>;

  const row = mount(jsx);

  expect(validator.isURL(row.find('a').props().href, { require_host: false, })).toBeTruthy();
});

it('does not have button when undefined', () => {
  const jsx =
    <Router>
      <table>
        <tbody>
          <SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", }} />
        </tbody>
      </table>
    </Router>;

  const row = mount(jsx);

  expect(row.find('td')).toHaveLength(2);
});

it('does have button when defined', () => {
  const jsx =
    <Router>
      <table>
        <tbody>
          <SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", }} buttonData={{ body: "example" }} />
        </tbody>
      </table>
    </Router>;

  const row = mount(jsx);

  expect(row.find('td')).toHaveLength(3);
});

it('calls on click', () => {
  const callback = jest.fn();

  const jsx =
    <Router>
      <table>
        <tbody>
          <SessionRow sessionData={{ id: "EXEX", fullname: "Smitty Jensens", }} buttonData={{ body: "example", onClick: callback }} />
        </tbody>
      </table>
    </Router>;

  const row = mount(jsx);

  row.find('button').simulate('click');

  expect(callback).toHaveBeenCalledTimes(1);
});