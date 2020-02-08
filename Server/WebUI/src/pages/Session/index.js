import React, { Component } from 'react';

import Menu from 'components/Menu';
import Session from "components/SingleSession";

import { getData, postData, pollGet, } from "libs/fetchExtra";

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Session/style';

class SessionPage extends Component {
  constructor(props) {
    super(props)

    this.state = {
      session: null,
      isExported: false
    }

    this.exportCallback = this.onExport.bind(this);
    this.sessionCallback = this.getSession.bind(this);
    this.pollingCallback = this.pollExport.bind(this);
  }

  async getSession()
  {
    const { id } = this.props.match.params;

    const url = `/api/v1/sessions/${id}?ugly=true`;
    const getSessions = await getData(url);
    const sessionsData = await getSessions.json();

    // Flattening the structure
    const cleanedSession = {
      id: sessionsData.id,
      fullname: sessionsData.user.fullname,
      createdAt: sessionsData.createdAt.$date,
      isExported: sessionsData.isExported,
      isPending: sessionsData.isPending,
    }

    this.setState({
      session: cleanedSession,
    });
  }

  async onExport() {
    const { id } = this.props.match.params;
    const url = `/api/v1/sessions/${id}/export/`;

    try {
      const exportRequest = await postData(url);

      if (!exportRequest.ok) {
        throw Error(exportRequest.status);
      }

      this.pollingCallback();
      
      this.sessionCallback();
    } catch (err) {
      throw Error(err);
    }
  }

  async pollExport() {
    const { id } = this.props.match.params;
    const url = `/api/v1/sessions/${id}/export/`;
    await pollGet(url);

    this.sessionCallback();
  }

  async componentDidMount()
  {
    await this.getSession();

    if (this.state.session.isPending) {
      await this.pollingCallback();
    }
  }

  render() {
    if (this.state.session != null)
    {
      return (
        <Wrapper>
          <Container>
            <Menu />
            <Row className="mb-3">
              <Col>
                <Session
                  sessionData={this.state.session}
                  onExport={this.exportCallback}
                />
              </Col>
            </Row>
            <Row>
              <Col className="text-center">
                {this.state.session.isPending &&
                  <h6>Exporting...</h6>
                }
              </Col>
            </Row>
          </Container>
        </Wrapper>
      )
    } else {
      // TODO: Fix this so it don't show on reload
      return (
      <Wrapper>
        <p>Fetching session...</p>
      </Wrapper>
      )
    }
  }
}

export default SessionPage;