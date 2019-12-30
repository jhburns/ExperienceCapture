import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

import { deleteData } from 'libs/fetchExtra';

import { Link } from 'react-router-dom';

class ArchivedSessionsPage extends Component {
  constructor(props) {
    super(props);

    this.archiveCallback = this.onArchive.bind(this);
  }

  async onArchive(id) {
    try {
      const dearchiveRequest = await deleteData(`/api/v1/sessions/${id}/tags/archived`);

      if (!dearchiveRequest.ok) {
        throw Error(dearchiveRequest.status);
      }
    } catch (err) {
      console.error(err);
    }
  }

  render() {
    return (
      <div>
        <p>Welcome To Archived Sessions</p>
        <Menu />
        <SessionTable
          sessionsQuery={""}
          buttonData={{
            onClick: this.archiveCallback,
            body: "Unarchive",
            header: ""
          }}
          hasTag={"archived"}
        />
        <Link to="/home/sessions">Back</Link>
      </div>
    );
  }
}

export default ArchivedSessionsPage;
