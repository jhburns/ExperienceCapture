import React from 'react';
import { shallow } from 'enzyme';
import GoogleSignIn from 'components/GoogleSignIn';

// TODO: Add way more tests for this component
it('has non-empty content', () => {
  const wrapper = shallow(<GoogleSignIn />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});