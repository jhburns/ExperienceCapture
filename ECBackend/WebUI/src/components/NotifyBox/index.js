import React, { Component } from 'react';

import { Wrapper, Info } from 'components/NotifyBox/style';

import { H6 } from '@bootstrap-styled/v4';

class NotifyBox extends Component {
  render() {
    return (
      <Wrapper>
        <Info className="align-middle p-3">
          <H6 className="text-center mb-0">
            {this.props.children}
          </H6>
        </Info>
      </Wrapper>
    );
  }
}

export default NotifyBox;