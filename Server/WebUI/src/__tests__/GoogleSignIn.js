import React from 'react';
import { shallow } from 'enzyme';
import GoogleSignIn from 'components/GoogleSignIn';

// TODO: Add way more tests for this component
it('has non-empty content', () => {
  const message = shallow(<GoogleSignIn />);

  expect(message.text().length).toBeGreaterThan(0);
});