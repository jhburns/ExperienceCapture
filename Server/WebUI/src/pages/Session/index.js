import React, { Component } from 'react';

import Menu from 'components/Menu';
import SingleSession from "components/SingleSession";

import { getData, postData, } from "libs/fetchExtra";

import { Container, Row, Col, } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Session/style';

import Footer from "components/Footer";

class SessionPage extends Component {
  constructor(props) {
    super(props)

    this.state = {
      session: null,
    }

    this.exportCallback = this.onExport.bind(this);
    this.sessionCallback = this.getSession.bind(this);
    this.poll = this.pollSession.bind(this);
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
      isOpen: sessionsData.isOpen,
      isOngoing: sessionsData.isOngoing
    }

    return cleanedSession;
  }

  async onExport() {
    const { id } = this.props.match.params;
    const url = `/api/v1/sessions/${id}/export/`;

    try {
      const exportRequest = await postData(url);

      if (!exportRequest.ok) {
        throw Error(exportRequest.status);
      }
      
      const currentSession = await this.sessionCallback();
      this.setState({
        session: currentSession,
      });
    } catch (err) {
      throw Error(err);
    }
  }


  async pollSession() {
    const currentSession = await this.getSession();

    // Very hacky, but easiest way to do abstract comparisons
    if (JSON.stringify(currentSession) !== JSON.stringify(this.state.session)) {
      this.setState({
        session: currentSession
      });
    }
  }

  async componentDidMount()
  {
    const firstSessions = await this.getSession();

    this.setState({
      session: firstSessions,
    });

    this.poller = setInterval(() => this.poll(), 10000); // 10 seconds
  }

  componentWillUnmount() {
    clearInterval(this.poller);
    this.poller = null;
  }

  render() {
    if (this.state.session != null)
    {
      return (
        <Wrapper>
          <Container className="p-0">
            <Menu />
            <Row className="mb-3 p-5">
              <Col>
                <SingleSession
                  sessionData={this.state.session}
                  onExport={this.exportCallback}
                />
              </Col>
            </Row>
            <Row>
              <Col className="text-center">
                {this.state.session.isPending &&
                  <h5>Exporting...</h5>
                }
              </Col>
            </Row>
            <Footer />
          </Container>
        </Wrapper>
      )
    } else {
      // TODO: Fix the component so this loading screen doesn't need to be shown
      return (
        <Wrapper>
          <Container className="p-0">
            <Menu />
            <Row>
              <Col className="text-center">
                <h3>
                  Fetching session...
                </h3>
              </Col>
            </Row>
            <Footer />
          </Container>
        </Wrapper>
      )
    }
  }
}

export default SessionPage;
