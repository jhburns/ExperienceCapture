import React, { Component } from 'react';

import Menu from 'components/Menu';
import Session from "components/SingleSession";

import { getData, postData, pollGet  } from "libs/fetchExtra";

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
        <div>
          <Menu />
          <Session
            sessionData={this.state.session}
            onExport={this.exportCallback}
          />
          {this.state.session.isPending &&
            <p>Exporting...</p>
          }
        </div>
      );
    } else {
      return (
        <div>
          <p>404: Session not found</p>
        </div>
      );
    }
  }
}

export default SessionPage;
