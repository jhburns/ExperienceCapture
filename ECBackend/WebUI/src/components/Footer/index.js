import React, { Component } from 'react';

import { Wrapper, Item } from 'components/Footer/style';
import { Row, Col, H4 } from '@bootstrap-styled/v4';

class Footer extends Component {
  render() {
    // Only component that gets top spacing besides Menu
    // Because adding consistent bottom spacing would be harder to maintain
    return (
      <Wrapper>
        <Row className="pt-5 pl-3 pr-3" noGutters={true}>
          <Col className="pl-5 pb-2">
            <H4
              href="https://github.com/jhburns/ExperienceCapture/tree/master/Documentation#documentation"
              target="_blank" rel="noopener noreferrer"
              as={Item}
            >
              Help
            </H4>
          </Col>
          <Col>
            <H4 className="text-muted mb-0 pr-0">
              {`© ${new Date().getFullYear()}`}
            </H4>
          </Col>
          <div className="w-100"></div>
          <Col className="pl-5 pb-2">
            <H4
              href="/api/v1/openapi/ui/index.html"
              target="_blank" rel="noopener noreferrer"
              as={Item}
            >
              API
            </H4>
          </Col>
          <Col></Col>
          <div className="w-100"></div>
          <Col className="pl-5 pb-2">
            <H4
              href="https://github.com/jhburns/ExperienceCapture/"
              target="_blank" rel="noopener noreferrer"
              as={Item}
            >
              GitHub
            </H4>
          </Col>
          <Col></Col>
        </Row>
      </Wrapper>
    );
  }
}

export default Footer;