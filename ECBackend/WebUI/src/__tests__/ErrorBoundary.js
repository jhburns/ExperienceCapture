import React from 'react';
import { mount } from 'enzyme';
import ErrorBoundary from 'components/ErrorBoundary';

import { BrowserRouter as Router } from "react-router-dom";

it('passes content through itself', () => {
  const jsx =
    <Router>
      <ErrorBoundary>
        <p>test</p>
      </ErrorBoundary>
    </Router>;

  const wrapper = mount(jsx);

  expect(wrapper.text()).toBe("test");
});

it('handles an error', () => {
  const ErrorTarget = () => null;

  const jsx =
    <Router>
      <ErrorBoundary>
        <ErrorTarget />
      </ErrorBoundary>
    </Router>;

  const wrapper = mount(jsx);

  wrapper.find(ErrorTarget).simulateError(new Error('test'));
});