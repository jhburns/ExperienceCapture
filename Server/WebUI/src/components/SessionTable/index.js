import React, { Component } from 'react';

import { getData } from 'libs/fetchExtra';

import SessionRow from 'components/SessionRow';

import { P, Row, Col, } from '@bootstrap-styled/v4';
import { Wrapper } from 'components/SessionTable/style';

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
    
    this.setState({
      sessions: sessionsConverted
    });
  }

  render() {
    const items = []
    const isEmpty = () => items.length === 0;

    for (const [index, value] of this.state.sessions.entries()) {
      items.push(<SessionRow 
        key={index}
        sessionData={value} 
        buttonData={this.props.buttonData}
        isRenderingDate={this.props.isRenderingDate}
      />)
    }

    return (
      <Wrapper>
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
          <Row className="justify-content-center">
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