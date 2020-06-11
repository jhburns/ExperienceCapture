import React from 'react';
import { shallow, mount, } from 'enzyme';
import SessionTable from 'components/SessionTable';

it('has non-empty content', () => {
  const table = shallow(<SessionTable />);

  expect(table.text().length).toBeGreaterThan(0);
});

it('lacks button header when undefined', () => {
  const table = mount(<SessionTable />);

  expect(table.find('th')).toHaveLength(3);
});

it('has button header when defined', () => {
  const table = mount(<SessionTable buttonData={{}} />);

  expect(table.find('th')).toHaveLength(4);
});

it('uses empty text when items is empty', () => {
  const table = mount(<SessionTable sessions={[]} emptyMessage="example" />);

  expect(table.text().includes("example")).toBeTruthy();
});