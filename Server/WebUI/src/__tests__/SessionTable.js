import React from 'react';
import { shallow, mount, } from 'enzyme';
import SessionTable from 'components/SessionTable';

import { StaticRouter as Router } from "react-router-dom";
import validator from 'validator';

it('has non-empty content', () => {
  const row = shallow(<SessionTable />);

  expect(row.text().length).toBeGreaterThan(0);
});