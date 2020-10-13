import React, { Component } from 'react';

import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';

import { Wrapper, CopyText } from "components/GetSignUpLink/style";

import { Button, Row, Col } from '@bootstrap-styled/v4';

import { faCopy } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { colors } from 'libs/theme';

import { CopyToClipboard } from 'react-copy-to-clipboard';

class GetSignUpLink extends Component {
  constructor(props) {
    super(props);

    this.state = {
      link: "",
      error: null,
    };

    this.onButtonCLick = this.onButtonCLick.bind(this);
  }

  async onButtonCLick() {
    this.setState({ link: "" });

    const signUpRequest = await postData("/api/v1/authentication/signUps/", {});

    if (!signUpRequest.ok) {
      this.setState({ error: new Error(signUpRequest.status) });
    }

    const request = await signUpRequest.json();
    const query = queryString.stringify({ signUpToken: request.signUpToken });
    const source = window.location.origin;

    this.setState({
      link: `${source}/?${query}`,
    });
  }

  render() {
    if (this.state.error) {
      throw this.state.error;
    }

    return (
      <Wrapper>
        <Button onClick={this.onButtonCLick} data-cy="new-sign-up" size="lg">
          New Sign Up Link
        </Button>
        {this.state.link !== "" &&
          <Row className="mt-3">
            <Col xs={10}>
              <CopyText className="rounded p-1" data-cy="new-link">
                {this.state.link}
              </CopyText>
            </Col>
            <Col xs={2} className="align-self-center">
              {/* TODO: add tests for this button*/}
              <CopyToClipboard text={this.state.link}>
                <FontAwesomeIcon
                  icon={faCopy}
                  color={colors.primary}
                  data-cy="new-link-copy"
                />
              </CopyToClipboard>
            </Col>
          </Row>
        }
      </Wrapper>
    );
  }
}
export default GetSignUpLink;