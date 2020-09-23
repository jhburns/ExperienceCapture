import React from 'react';
import { shallow, mount } from 'enzyme';
import SingleSession from 'components/SingleSession';

import { StaticRouter as Router } from "react-router-dom";

it('has non-empty content', () => {
  const wrapper = shallow(<SingleSession sessionData={{ exportState: "Done", createdAt: 1591840213871 }} />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('is completed when done', () => {
  const jsx =
    <Router>
      <SingleSession sessionData={{ exportState: "NotStarted", isOngoing: false, isOpen: false, createdAt: 1591840213871 }} />
    </Router>;

  const wrapper = mount(jsx);

  expect(wrapper.text().includes("Completed")).toBeTruthy();
});

it('is ongoing when ongoing', () => {
  const jsx =
    <Router>
      <SingleSession sessionData={{ exportState: "NotStarted", isOngoing: true, isOpen: true, createdAt: 1591840213871 }} />
    </Router>;

  const wrapper = mount(jsx);

  expect(wrapper.text().includes("Ongoing")).toBeTruthy();
});

it('is closed unexpectedly when not ongoing but still open', () => {
  const jsx =
    <Router>
      <SingleSession sessionData={{ exportState: "NotStarted", isOngoing: false, isOpen: true, createdAt: 1591840213871 }} />
    </Router>;

  const wrapper = mount(jsx);

  expect(wrapper.text().includes("Closed Unexpectedly")).toBeTruthy();
});