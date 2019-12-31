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
    const url = `/api/v1/sessions?${this.props.sessionsQuery}&ugly=true`;
    const getSessions = await getData(url);
    const sessionsData = await getSessions.json();
    const sessions = sessionsData.contentArray;

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

    const sessionsConverted = sessionsFiltered.map((s) => {
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
      items.push(<Session 
        key={index}
        sessionData={value} 
        buttonData={this.props.buttonData}
        isRenderingDate={this.props.isRenderingDate}
      />)
    }

    return (
      <table className="table">
        <thead>
          <tr>
            <th scope="col">ID</th>
            <th scope="col">Captured By</th>
            <th scope="col">{this.props.isRenderingDate ? "Date" : "Time"}</th>
            {this.props.buttonData !== undefined &&
              <th scope="col">{this.props.buttonData.header}</th>
            }
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