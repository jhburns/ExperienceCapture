import React from 'react';
import { shallow, mount } from 'enzyme';
import SessionTable from 'components/SessionTable';

it('has non-empty content', () => {
  const wrapper = shallow(<SessionTable queryOptions={{}} />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('lacks button header when undefined', () => {
  const wrapper = mount(<SessionTable queryOptions={{}} />);

  expect(wrapper.find('th')).toHaveLength(3);
});

it('has button header when defined', () => {
  const wrapper = mount(<SessionTable queryOptions={{}} buttonData={{}} />);

  expect(wrapper.find('th')).toHaveLength(4);
});

it('uses empty text when items is empty', () => {
  const wrapper = mount(<SessionTable queryOptions={{}} sessions={[]} emptyMessage="example" />);

  expect(wrapper.text().includes("example")).toBeTruthy();
});