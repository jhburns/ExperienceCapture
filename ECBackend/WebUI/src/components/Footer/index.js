import React, { Component } from 'react';

import { Wrapper, Item } from 'components/Footer/style';
import { Row, Col, H4 } from '@bootstrap-styled/v4';

class Footer extends Component {
  render() {
    // Only component that gets top spacing besides Menu
    // Because adding consistent bottom spacing would be harder to maintain
    return (
      <Wrapper>
        {/* Don't add top padding to this or stickiness will break */}
        <Row className="pl-5 pr-5 justify-content-end" noGutters={true}>
          <Col xs={6} lg={4}>
            <Row className="justify-content-center">
              <Col xs={12} lg="auto" className="pl-3 pb-2">
                <Item
                  href="https://github.com/jhburns/ExperienceCapture/tree/master/Documentation#documentation"
                  target="_blank" rel="noopener noreferrer"
                >
                  Help
                </Item>
              </Col>
              <Col xs={12} lg="auto" className="pl-3 pb-2">
                <Item
                  href="/api/v1/openapi/ui/index.html"
                  target="_blank" rel="noopener noreferrer"
                >
                  API
                </Item>
              </Col>
              <Col xs={12} lg="auto" className="pl-3 pb-2">
                <Item
                  href="https://github.com/jhburns/ExperienceCapture/"
                  target="_blank" rel="noopener noreferrer"
                >
                  GitHub
                </Item>
              </Col>
            </Row>
          </Col>
          <Col xs={6} lg={4}>
            <H4 className="pl-3 text-muted mb-0 pr-0 text-center">
              {`© ${new Date().getFullYear()}`}
            </H4>
          </Col>
        </Row>
      </Wrapper>
    );
  }
}

export default Footer;