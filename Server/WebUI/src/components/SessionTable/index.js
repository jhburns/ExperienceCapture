import React, { Component } from 'react';

import { getData } from 'libs/fetchExtra';

import SessionRow from 'components/SessionRow';

import { P, Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper } from 'components/SessionTable/style';

import { postData, deleteData, wait, } from 'libs/fetchExtra';

// TODO: fix this so selecting archive/unarchive results in state/page refresh
class SessionTable extends Component {
  constructor(props) {
    super(props)

    this.state = {
      sessions: [],
    }

    this.tagCallback = this.onTag.bind(this);
    this.getSessionCallback = this.onSession.bind(this);
    this.poll = this.pollSessions.bind(this);
  }

  async onTag(id) {
    try {
      let archiveRequest;
      if (this.props.buttonData.isAdd) {
        archiveRequest = await postData(`/api/v1/sessions/${id}/tags/archived`);
      } else {
        archiveRequest = await deleteData(`/api/v1/sessions/${id}/tags/archived`);
      }

      if (!archiveRequest.ok) {
        throw Error(archiveRequest.status);
      }

      const sessionsUpdated = this.state.sessions.filter((session) => {
        return session.id !== id;
      });

      this.setState({
        sessions: sessionsUpdated
      });
    } catch (err) {
      console.error(err);
    }
  }

  async pollSessions() {
    while (true) {
      const currentSessions = await this.getSessionCallback();

      // Very hacky, but easiest way to do abstract comparisons
      if (JSON.stringify(currentSessions) !== JSON.stringify(this.state.sessions)) {
        this.setState({
          sessions: currentSessions
        });
      }

      await wait(5000); // 5 seconds
    }
  }

  async onSession() {
    const url = `/api/v1/sessions?${this.props.sessionsQuery}&ugly=true`;
    const getSessions = await getData(url);
    const sessionsData = await getSessions.json();
    const sessions = sessionsData.contentArray;

    // Removing any session if it lacks or has a tag it shouldn't
    const sessionsFiltered = sessions.reduce((sessions, s) => {
      if (this.props.lacksTag !== undefined || this.props.hasTag !== undefined) {
        if (this.props.lacksTag !== undefined && !s.tags.includes(this.props.lacksTag)) {
          sessions.push(s);
        } else if (this.props.hasTag !== undefined && s.tags.includes(this.props.hasTag)) {
          sessions.push(s);
        }
      } else {
        sessions.push(s);
      }

      return sessions;
    }, []);

    // Removing all the extra data from each session, and flattening
    const sessionsConverted = sessionsFiltered.map((s) => {
      return {
        id: s.id,
        fullname: s.user.fullname,
        createdAt: s.createdAt.$date
      }
    });

    return sessionsConverted;
  }

  async componentDidMount() {
    const firstSessions = await this.getSessionCallback();
    this.setState({
      sessions: firstSessions
    });

    this.poll();
  }

  render() {
    const items = []
    const isEmpty = () => items.length === 0;

    for (const [index, value] of this.state.sessions.entries()) {
      items.push(<SessionRow 
        key={index}
        sessionData={value} 
        buttonData={{
          body: this.props.buttonData.body,
          onClick: this.tagCallback
        }}
        isRenderingDate={this.props.isRenderingDate}
      />)
    }

    return (
      <Wrapper>
        <h2 className="mb-3 pl-3 pl-lg-0">
          {this.props.title}
        </h2>
        <table className="table mb-5">
          <thead className="thead-dark">
            <tr>
              <th scope="col m-0">ID</th>
              <th scope="col">Captured By</th>
              <th scope="col">{this.props.isRenderingDate ? "Date" : "Time" }</th>
              {this.props.buttonData !== undefined &&
                <th scope="col">{this.props.buttonData.header}</th>
              }
            </tr>
          </thead>
          <tbody>
            {items}
          </tbody>
        </table>
        
        {isEmpty() &&
          <Row className="justify-content-center mb-5">
            <Col>
              <P className="text-center">{this.props.emptyMessage}</P>
            </Col>
          </Row>
        }
      </Wrapper>
    )
  }
}

export default SessionTable;