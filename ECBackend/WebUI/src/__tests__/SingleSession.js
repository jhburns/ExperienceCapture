import React from 'react';
import { shallow, mount, } from 'enzyme';
import SingleSession from 'components/SingleSession';

import { StaticRouter as Router } from "react-router-dom";

it('has non-empty content', () => {
  const session = shallow(<SingleSession sessionData={{ exportState: "Done" }} />);

  expect(session.text().length).toBeGreaterThan(0);
});

it('is completed when done', () => {
  const jsx =
    <Router>
      <SingleSession sessionData={{ exportState: "NotStarted", isOngoing: false, isOpen:false }} />
    </Router>;
  const session = mount(jsx);

  expect(session.text().includes("Completed")).toBeTruthy();
});

it('is ongoing when ongoing', () => {
  const jsx =
    <Router>
      <SingleSession sessionData={{ exportState: "NotStarted", isOngoing: true, isOpen: true }} />
    </Router>;
  const session = mount(jsx);

  expect(session.text().includes("Ongoing")).toBeTruthy();
});

it('is closed unexpectedly when not ongoing but still open', () => {
  const jsx =
    <Router>
      <SingleSession sessionData={{ exportState: "NotStarted", isOngoing: false, isOpen: true }} />
    </Router>;
  const session = mount(jsx);

  expect(session.text().includes("Closed Unexpectedly")).toBeTruthy();
});