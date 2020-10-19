import React from 'react';
import { mount } from 'enzyme';
import NotifyBox from 'components/NotifyBox';

it('has non-empty content', () => {
  const wrapper = mount(<NotifyBox>Test</NotifyBox>);

  expect(wrapper.text().length).toBeGreaterThan(0);
});