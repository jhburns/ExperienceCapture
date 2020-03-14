import React, { Component } from 'react';

import { Wrapper, Item } from 'components/Footer/style';
import { Row, Col, } from '@bootstrap-styled/v4';

class Footer extends Component {
  render() {
    // Only component that gets top spacing besides Menu
    // Because adding consistent bottom spacing would be harder to maintain
    return (
      <Wrapper className="mt-5">
        <Row className="m-0">
          <Col className="text-center">
            <Item
              href="https://github.com/jhburns/ExperienceCapture/tree/master/Documentation#documentation"
              target="_blank" rel="noopener noreferrer"
            >
              Help
            </Item>
          </Col>
        </Row>
      </Wrapper>
    )
  }
}

export default Footer;