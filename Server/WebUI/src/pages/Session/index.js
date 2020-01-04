import React, { Component } from 'react';

import Menu from 'components/Menu';
import Session from "components/Session";

import { getData } from "libs/fetchExtra";

class SessionPage extends Component {
  constructor(props) {
    super(props)

    this.state = {
      session: null,
      isExported: false
    }

    this.downloadCallback = this.onDownload.bind(this);
    this.exportCallback = this.onExport.bind(this);
  }

  async onExport() {
    
  }

  async componentDidMount()
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
