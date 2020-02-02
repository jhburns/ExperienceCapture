import React, { Component } from 'react';

import moment from 'moment';

import { Link } from 'react-router-dom';

import { Row, } from '@bootstrap-styled/v4';

class SingleSession extends Component {
  render() {
    const isExportDisabled = this.props.sessionData.isPending || this.props.sessionData.isExported;

    return (
      <div>
        <Row><p>Session: {this.props.sessionData.id}</p></Row>
        <p>Captured By: {this.props.sessionData.fullname}</p>
        <p>Date: {moment(this.props.sessionData.createdAt).format("MMM Do YY hh:mm a")}</p>
        <button
          onClick={this.props.onExport}
          disabled={isExportDisabled}
        >
          Export
        </button>
        <Link 
          to={`/api/v1/sessions/${this.props.sessionData.id}/export/`}
          target="_blank" 
          download
        >
          Download
        </Link>
      </div>
    )
  }
}

export default SingleSession;