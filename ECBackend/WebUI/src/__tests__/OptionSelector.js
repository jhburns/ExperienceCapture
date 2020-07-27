import React from 'react';
import { shallow, mount } from 'enzyme';
import About from 'components/OptionSelector';
import OptionSelector from 'components/OptionSelector';

it('has non-empty content', () => {
  const wrapper = shallow(<OptionSelector options={["test"]} />);

  expect(wrapper.text().length).toBeGreaterThan(0);
});

it('exists', () => {
  const wrapper = mount(<OptionSelector options={["test"]} />);

  expect(wrapper.find('button').exists()).toBeTruthy();
});

it('has an initial state of closed', () => {
  const wrapper = mount(<About options={["test"]} />);

  expect(wrapper.state().isOpen).toBe(false);
});

it('calls on click', () => {
  const callback = jest.fn();

  const wrapper = mount(<OptionSelector options={["test"]} onClick={callback} />);

  wrapper.find('button').forEach((node) => {
    node.simulate('click');
  });

  expect(callback).toHaveBeenCalledTimes(1);
});

it('has at least enough buttons', () => {
  const testOptions = ["test1", "test2", "test3", "test4", "test5"];

  const wrapper = mount(<OptionSelector options={testOptions} />);

  expect(wrapper.find('button').length).toBeGreaterThan(testOptions.length);
});