import React from 'react';
import { shallow } from 'enzyme';
import Tutorial from 'components/Tutorial';

import validator from 'validator';

it('has non-empty content', () => {
  const tutorial = shallow(<Tutorial />);

  expect(tutorial.text().length).toBeGreaterThan(0);
});

it('has valid links', () => {
  const tutorial = shallow(<Tutorial />);

  tutorial.find('a').forEach((node) => {
    if (!node.props().href.includes('localhost'))
    {
      expect(validator.isURL(node.props().href)).toBeTruthy();
    }
  });
});