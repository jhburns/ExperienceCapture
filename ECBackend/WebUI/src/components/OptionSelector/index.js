import React, { Component } from 'react';

import { Wrapper } from 'components/OptionSelector/style';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from '@bootstrap-styled/v4';

import { Row, Col, P } from '@bootstrap-styled/v4';

class OptionSelector extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: false,
      title: "",
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
            this.setState({ isOpen: false, title: value });
          }}
          data-cy={`session-sort-${value.replace(' ', '-')}`}
        >
          {value}
        </DropdownItem>);
    }

    return (
      <Wrapper>
        <Row>
          <Col>
            <P className="font-weight-medium mb-1">Sort By</P>
          </Col>
        </Row>
        <Row>
          <Col>
            <Dropdown
              isOpen={this.state.isOpen}
              toggle={() => this.setState({ isOpen: !this.state.isOpen })}
            >
              <DropdownToggle caret data-cy="sort-dropdown">
                {this.state.title === "" ? this.props.default : this.state.title}
              </DropdownToggle>
              <DropdownMenu>
                {items}
              </DropdownMenu>
            </Dropdown>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default OptionSelector;