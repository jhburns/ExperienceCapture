import React, { Component } from 'react';

import moment from 'moment';

import { Link } from 'react-router-dom';

class Session extends Component {
  render() {
    const isExportDisabled = this.props.sessionData.isPending || this.props.sessionData.isExported;
    console.log(isExportDisabled);
    return (
      <div>
        <p>Session: {this.props.sessionData.id}</p>
        <br />
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

  componentDidUpdate(prevProps, prevState) {
    Object.entries(this.props).forEach(([key, val]) =>
      prevProps[key] !== val && console.log(`Prop '${key}' changed`)
    );
    if (this.state) {
      Object.entries(this.state).forEach(([key, val]) =>
        prevState[key] !== val && console.log(`State '${key}' changed`)
      );
    }
  }
}

export default Session;