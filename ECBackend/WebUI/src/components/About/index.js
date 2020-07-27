import React, { Component } from 'react';

import { Wrapper, Prompt } from 'components/About/style';

import { Tooltip } from '@bootstrap-styled/v4';

class About extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: false,
    };

  }

  render() {
    return (
      <Wrapper className="pl-3 pr-3 d-inline-block">
        <Prompt
          className="d-inline-block"
          id="about-tooltip"
          data-cy="about-prompt"
        >
          ?
        </Prompt>
        <Tooltip
          placement="right"
          target="about-tooltip"
          isOpen={this.state.isOpen}
          toggle={() => {
            this.setState({
              isOpen: !this.state.isOpen,
            });
          }}
          data-cy="about-tooltip"
        >
          {this.props.message}
        </Tooltip>
      </Wrapper>
    );
  }
}
export default About;