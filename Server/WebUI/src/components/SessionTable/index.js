import React, { Component } from 'react';

import { getData } from 'libs/fetchExtra';

import Session from 'components/Session';

class SessionTable extends Component {
  constructor(props) {
    super(props)

    this.state = {
      sessions: [],
    }
  }

  async componentDidMount() {
    const url = `/api/v1/sessions?${this.props.sessionsQuery}`;
    const getSessions = await getData(url);
    const sessionsData = await getSessions.json();
    const sessions = sessionsData.contentArray;

    const sessionsConverted = sessions.map((s) => {
      return {
        id: s.id,
        fullname: s.user.fullname,
        createdAt: s.createdAt.$date
      } 
    });
    
    this.setState({
      sessions: sessionsConverted
    });
  }

  render() {
    const items = []

    for (const [index, value] of this.state.sessions.entries()) {
      items.push(<Session key={index} sessionData={value} />)
    }

    return (
      <table className="table">
        <thead>
          <tr>
            <th scope="col">ID</th>
            <th scope="col">Captured By</th>
            <th scope="col">Time</th>
          </tr>
        </thead>
        <tbody>
          {items}
        </tbody>
      </table>
    )
  }
}

export default SessionTable;