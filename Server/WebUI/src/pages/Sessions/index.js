import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { postData } from 'libs/fetchExtra';

import { Link } from 'react-router-dom';

class SessionsPage extends Component {
  constructor(props) {
    super(props);

    this.archiveCallback = this.onArchive.bind(this);
  }

  async onArchive(id) {
    try {
      const archiveRequest = await postData(`/api/v1/sessions/${id}/tags/archived`);

      if (!archiveRequest.ok) {
        throw Error(archiveRequest.status);
      }
    } catch (err) {
      console.error(err);
    }
  }

  render() {
    return (
      <div>
        <p>Welcome Home</p>
        <Menu />
        <SessionTable
          sessionsQuery={""} 
          buttonData={{
            onClick: this.archiveCallback,
            body: "Archive",
            header: ""
          }}
          lacksTag={"archived"}
          isRenderingDate={true}
      />
        <Link to="/home/archived">Archived</Link>
      </div>
    );
  }
}

export default SessionsPage;
