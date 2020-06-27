import React from 'react';
import { shallow, mount, } from 'enzyme';
import SessionTable from 'components/SessionTable';

it('has non-empty content', () => {
  const table = shallow(<SessionTable queryOptions={{}} />);

  expect(table.text().length).toBeGreaterThan(0);
});

it('lacks button header when undefined', () => {
  const table = mount(<SessionTable queryOptions={{}} />);

  expect(table.find('th')).toHaveLength(3);
});

it('has button header when defined', () => {
  const table = mount(<SessionTable queryOptions={{}} buttonData={{}} />);

  expect(table.find('th')).toHaveLength(4);
});

it('uses empty text when items is empty', () => {
  const table = mount(<SessionTable queryOptions={{}} sessions={[]} emptyMessage="example" />);

  expect(table.text().includes("example")).toBeTruthy();
});