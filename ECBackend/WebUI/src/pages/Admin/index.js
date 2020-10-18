import React, { Component } from 'react';
import queryString from 'query-string';

import { postData } from 'libs/fetchExtra';
import { Redirect } from 'react-router-dom';

import { Wrapper } from 'pages/Admin/style';

import { P, Container, Row, Col } from '@bootstrap-styled/v4';

class Admin extends Component {

  constructor(props) {
    super(props);
    this.state = {
      isError: false,
      isWaiting: true,
      accessToken: null,
    };

    this.onSuccess = this.onSuccess.bind(this);
    this.onError = this.onError.bind(this);
  }

  onSuccess(response) {
    this.setState({
      accessToken: response.signUpToken,
      isWaiting: false,
    });
  }

  onError() {
    this.setState({
      isError: true,
      isWaiting: false,
    });
  }

  async componentDidMount() {
    const query = queryString.parse(this.props.location.search);

    try {
      const data = {
        password: query.password,
      };

      const reply = await postData("/api/v1/authentication/admins/", data);

      if (reply.ok) {
        this.onSuccess(await reply.json());
      } else {
        this.onError();
        throw Error(reply.error);
      }
    } catch (error) {
      this.onError();
      // eslint-disable-next-line no-console
      console.error(error);
    }
  }

  render() {
    if (this.state.isWaiting) {
      return (
        <Wrapper>
          <Container>
            <Row className="justify-content-center">
              <Col className="mt-5">
                <P>Trying to use the supplied admin password to generate a sign up token.</P>
                <P>One sec...</P>
              </Col>
            </Row>
          </Container>
        </Wrapper>
      );
    } else if (this.state.isError) {
      return (
        <Wrapper>
          <Container>
            <Row className="justify-content-center">
              <Col className="mt-5">
                <P>Password is invalid.</P>
                <P>Check the console.</P>
              </Col>
            </Row>
          </Container>
        </Wrapper>
      );
    } else {
      return (
        <Wrapper>
          <Redirect
            to={{
              pathname: "/",
              search: queryString.stringify({signUpToken: this.state.accessToken}),
            }}
          />
        </Wrapper>
      );
    }
  }
}

export default Admin;
