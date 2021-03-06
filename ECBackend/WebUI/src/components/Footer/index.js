import React, { Component } from 'react';

import { Wrapper, Item } from 'components/Footer/style';
import { Row, Col, H4, Container } from '@bootstrap-styled/v4';

class Footer extends Component {
  render() {
    // Only component that gets top spacing besides Menu
    // Because adding consistent bottom spacing would be harder to maintain
    return (
      <Wrapper>
        {/* Don't add top padding to this or stickiness will break */}
        <Container>
          <Row className="pl-5 pr-5 justify-content-center" noGutters={true}>
            <Col xs={0} lg={4}></Col>
            <Col xs={6} lg={4}>
              <Row className="justify-content-center">
                <Col xs={12} lg="auto" className="pl-3 pb-2">
                  <Item
                    href="https://github.com/jhburns/ExperienceCapture/tree/master/Documentation#documentation"
                    target="_blank" rel="noopener noreferrer"
                    className="font-weight-medium"
                  >
                    Help
                  </Item>
                </Col>
                <Col xs={12} lg="auto" className="pl-3 pb-2">
                  <Item
                    href="/api/v1/openapi/ui/index.html"
                    target="_blank" rel="noopener noreferrer"
                    className="font-weight-medium"
                  >
                    API
                  </Item>
                </Col>
                <Col xs={12} lg="auto" className="pl-3 pb-2">
                  <Item
                    href="https://github.com/jhburns/ExperienceCapture/"
                    target="_blank" rel="noopener noreferrer"
                    className="font-weight-medium"
                  >
                    GitHub
                  </Item>
                </Col>
              </Row>
            </Col>
            <Col xs={6} lg={4}>
              <H4 className="text-muted mb-0 text-center">
                {`© ${new Date().getFullYear()}`}
              </H4>
            </Col>
          </Row>
        </Container>
      </Wrapper>
    );
  }
}

export default Footer;