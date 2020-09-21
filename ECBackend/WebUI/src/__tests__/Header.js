import React from 'react';
import { shallow } from 'enzyme';
import Header from 'components/Header';

it('has non-empty content', () => {
  const wrapper = shallow(<Header />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});