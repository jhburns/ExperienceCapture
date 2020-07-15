import React, { Component } from 'react';

import { Wrapper, Item } from 'components/Footer/style';
import { Row, Col, P, Container, } from '@bootstrap-styled/v4';

class Footer extends Component {
  render() {
    // Only component that gets top spacing besides Menu
    // Because adding consistent bottom spacing would be harder to maintain
    return (
      <Wrapper >
        <Container>
          <Row className="m-0">
            <Col className="text-center">
              <Item
                href="https://github.com/jhburns/ExperienceCapture/tree/master/Documentation#documentation"
                target="_blank" rel="noopener noreferrer"
              >
                HELP
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
                GITHUB
            </Item>
            </Col>
            <Col>
              <P className="text-muted">
                {`© ${new Date().getFullYear()}`}
              </P>
            </Col>
          </Row>
        </Container>
      </Wrapper>
    )
  }
}

export default Footer;