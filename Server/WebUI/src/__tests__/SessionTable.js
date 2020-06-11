import React from 'react';
import { shallow, mount, } from 'enzyme';
import SessionTable from 'components/SessionTable';

it('has non-empty content', () => {
  const row = shallow(<SessionTable />);

  expect(row.text().length).toBeGreaterThan(0);
});

it('lacks button header when undefined', () => {
  const row = mount(<SessionTable />);

  expect(row.find('th')).toHaveLength(3);
});

it('has button header when defined', () => {
  const row = mount(<SessionTable buttonData={{}} />);

  expect(row.find('th')).toHaveLength(4);
});

it('uses empty text when items is empty', () => {
  const row = mount(<SessionTable sessions={[]} emptyMessage="example" />);

  expect(row.text().includes("example")).toBeTruthy();
});