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

    this.onExport = this.onExport.bind(this);
    this.getSession = this.getSession.bind(this);
    this.poll = this.poll.bind(this);
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
      exportState: sessionsData.exportState,
      isOpen: sessionsData.isOpen,
      isOngoing: sessionsData.isOngoing
    }

    // Poll based on session state
    if (["Done", "NotStarted", "Error"].includes(cleanedSession.exportState)) {
      clearInterval(this.poller);
      this.poller = setInterval(() => this.poll(), 10000); // 10 seconds
    } else {
      clearInterval(this.poller);
      this.poller = setInterval(() => this.poll(), 250); // 0.25 seconds
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
      
      const currentSession = await this.getSession();
      this.setState({
        session: currentSession,
      });
    } catch (err) {
      throw Error(err);
    }
  }


  async poll() {
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
  }

  componentWillUnmount() {
    clearInterval(this.poller);
    this.poller = null;
  }

  render() {
    if (this.state.session === null)
    {
      return null;
    }

    return (
      <Wrapper>
        <Container className="p-0">
          <Menu />
          <Row className="mb-3 p-5">
            <Col>
              <SingleSession
                sessionData={this.state.session}
                onExport={this.onExport}
              />
            </Col>
          </Row>
          <Row>
            <Col className="text-center">
              {this.state.session.exportState === "Pending" &&
                <h5>Exporting...</h5>
              }
            </Col>
          </Row>
          <Footer />
        </Container>
      </Wrapper>
    )
  }
}

export default SessionPage;
