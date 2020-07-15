import React, { Component } from 'react';

import { Wrapper, Item } from 'components/Footer/style';
import { Row, Col, P, } from '@bootstrap-styled/v4';

class Footer extends Component {
  render() {
    // Only component that gets top spacing besides Menu
    // Because adding consistent bottom spacing would be harder to maintain
    return (
      <Wrapper >
        <Row>
          <Col className="text-center">
            <Item
              href="https://github.com/jhburns/ExperienceCapture/tree/master/Documentation#documentation"
              target="_blank" rel="noopener noreferrer"
            >
              Help
          </Item>
          </Col>
          <Col className="text-center">
            <Item
              href="/api/v1/openapi/ui/index.html"
              target="_blank" rel="noopener noreferrer"
            >
              API
          </Item>
          </Col>
          <Col className="text-center">
            <Item
              href="https://github.com/jhburns/ExperienceCapture/"
              target="_blank" rel="noopener noreferrer"
            >
              GitHub
          </Item>
          </Col>
          <Col>
            <P className="text-muted">
              {`© ${new Date().getFullYear()}`}
            </P>
          </Col>
        </Row>
      </Wrapper>
    )
  }
}

export default Footer;