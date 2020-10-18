import React from 'react';
import { mount } from 'enzyme';
import Brand from 'components/Brand';

import { BrowserRouter as Router } from "react-router-dom";

it('has a link', () => {
  const wrapper = mount(<Router><Brand to="test" /></Router>);

  expect(wrapper.find('a').exists()).toBeTruthy();
});