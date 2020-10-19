import React, { Component } from 'react';

import Menu from 'components/Menu';
import SingleSession from "components/SingleSession";

import { getData, postData } from "libs/fetchExtra";

import { Container, Row, Col, A } from '@bootstrap-styled/v4';

import { Wrapper } from 'pages/Session/style';

import Footer from "components/Footer";

class SessionPage extends Component {
  constructor(props) {
    super(props);

    this.state = {
      session: null,
      error: null,
    };

    this.onExport = this.onExport.bind(this);
    this.getSession = this.getSession.bind(this);
    this.poll = this.poll.bind(this);
  }

  async getSession()
  {
    const { id } = this.props.match.params;

    const url = `/api/v1/sessions/${id}?ugly=true`;

    try {
      const getSessions = await getData(url);
      if (!getSessions.ok) {
        this.setState({ error: new Error(getSessions.status) });
        return;
      }

      const sessionData = await getSessions.json();

      // Flattening the structure
      const cleanedSession = {
        id: sessionData.id,
        fullname: sessionData.user.fullname,
        createdAt: parseInt(sessionData.createdAt.$date.$numberLong),
        exportState: sessionData.exportState,
        isOpen: sessionData.isOpen,
        isOngoing: sessionData.isOngoing,
      };

      // Poll based on session state
      if (["Done", "NotStarted", "Error"].includes(cleanedSession.exportState)) {
        clearInterval(this.poller);
        this.poller = setInterval(() => this.poll(), 10000); // 10 seconds
      } else {
        clearInterval(this.poller);
        this.poller = setInterval(() => this.poll(), 250); // 0.25 seconds
      }

      return cleanedSession;
    } catch(err) {
      this.setState({ error: err });
      return;
    }
  }

  async onExport() {
    const { id } = this.props.match.params;
    const url = `/api/v1/sessions/${id}/export/`;

    try {
      const exportRequest = await postData(url);

      if (!exportRequest.ok) {
        this.setState({ error: new Error(exportRequest.status) });
        return;
      }

      const currentSession = await this.getSession();
      this.setState({
        session: currentSession,
      });
    } catch (err) {
      this.setState({ error: err });
      return;
    }
  }


  async poll() {
    const currentSession = await this.getSession();

    // Very hacky, but easiest way to do abstract comparisons
    if (JSON.stringify(currentSession) !== JSON.stringify(this.state.session)) {
      this.setState({
        session: currentSession,
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
    if (this.state.error !== null) {
      throw this.state.error;
    }

    return (
      <Wrapper>
        <Menu />
        <Container className="pb-5 mt-5">
          {this.state.session !== null &&
            <>
              <Row className="justify-content-center mb-1">
                <Col xs={10} lg={8}>
                  <A href="#" onClick={() => this.props.history.goBack()}>&lt; Back</A>
                </Col>
              </Row>
              <SingleSession sessionData={this.state.session} onExport={this.onExport} />
            </>
          }
        </Container>
        <Footer />
      </Wrapper>
    );
  }
}

export default SessionPage;
