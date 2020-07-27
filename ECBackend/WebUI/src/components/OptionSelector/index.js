import React, { Component } from 'react';

import { Wrapper } from 'components/OptionSelector/style';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from '@bootstrap-styled/v4';

class OptionSelector extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: false,
    };
  }

  render() {
    const items = [];

    for (const [index, value] of this.props.options.entries()) {
      items.push(
        <DropdownItem
          key={index}
          onClick={() => {
            this.props.onClick(value);
            this.setState({ isOpen: false });
          }}
          data-cy={`session-sort-${value.replace(' ', '-')}`}
        >
          {value}
        </DropdownItem>);
    }

    return (
      <Wrapper>
        <Dropdown
          isOpen={this.state.isOpen}
          toggle={() => this.setState({ isOpen: !this.state.isOpen })}
        >
          <DropdownToggle caret data-cy="sort-dropdown">
            {this.props.title}
          </DropdownToggle>
          <DropdownMenu>
            {items}
          </DropdownMenu>
        </Dropdown>
      </Wrapper>
    );
  }
}

export default OptionSelector;