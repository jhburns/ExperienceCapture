import React, { Component } from 'react';

import { getData } from 'libs/fetchExtra';

import SessionRow from 'components/SessionRow';

import { P, Row, Col, Button, } from '@bootstrap-styled/v4';
import { Wrapper, } from 'components/SessionTable/style';

import { postData, deleteData, } from 'libs/fetchExtra';

import queryString from 'query-string';

class SessionTable extends Component {
  constructor(props) {
    super(props)

    this.state = {
      sessions: [],
      isAllowedToPoll: true,
      pageNumber: 1,
      pageTotal: 0,
    }

    this.onTag = this.onTag.bind(this);
    this.getSessions = this.getSessions.bind(this);
    this.updateSessions = this.updateSessions.bind(this);
    this.navigatePages = this.navigatePages.bind(this);
    this.poll = this.poll.bind(this);

    this.topReference = React.createRef();
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

      await this.getSessions();
    } catch (err) {
      console.error(err);
    }
  }

  async poll() {
    await this.updateSessions();
  }

  async updateSessions() {
    const sessions = await this.getSessions(this.state.pageNumber);

    // Very hacky, but easiest way to do abstract comparisons
    if (JSON.stringify(sessions) !== JSON.stringify(this.state.sessions)
      || this.state.pageTotal !== sessions.pageTotal) {
      this.setState(sessions);
    }
  }

  async getSessions(page) {
    let queryOptions = this.props.queryOptions;
    queryOptions.ugly = true;
    queryOptions.page = page;
    const query = queryString.stringify(queryOptions);

    const url = `/api/v1/sessions?${query}`;
    const getSessions = await getData(url);
    const sessionsData = await getSessions.json();
    const sessions = sessionsData.contentArray;

    // Removing all the extra data from each session, and flattening
    const sessionsConverted = sessions.map((s) => {
      return {
        id: s.id,
        fullname: s.user.fullname,
        createdAt: s.createdAt.$date
      }
    });

    return {
      sessions: sessionsConverted,
      pageTotal: sessionsData.pageTotal,
    };
  }

  async navigatePages(change) {
    const nextPage = this.state.pageNumber + change;
    const sessions = await this.getSessions(nextPage);

    this.setState(Object.assign(sessions, { pageNumber: nextPage }));

    window.scrollTo(0, this.topReference.current.offsetTop);
  }

  async componentDidMount() {
    await this.updateSessions();

    this.poller = setInterval(() => this.poll(), 10000); // 10 seconds
  }

  componentWillUnmount() {
    clearInterval(this.poller);
    this.poller = null;
  }

  render() {
    const items = []
    const isEmpty = () => items.length === 0;

    for (const [index, value] of this.state.sessions.entries()) {
      items.push(<SessionRow 
        key={index}
        sessionData={value} 
        buttonData={this.props.buttonData !== undefined ? {
          body: this.props.buttonData.body,
          onClick: this.onTag
        } : undefined}
        isRenderingDate={this.props.isRenderingDate}
      />)
    }

    return (
      <Wrapper className="mb-5" ref={this.topReference}>
        <h2 className="mb-3 pl-3 pl-lg-0" id="start-of-sessions">
          {this.props.title}
        </h2>
        <table className="table mb-4">
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
          <Row className="m-0 justify-content-center mb-4">
            <Col>
              <P className="text-center">{this.props.emptyMessage}</P>
            </Col>
          </Row>
        }
        <Row>
          <Col className="text-center">
            <Button
              className="mr-2"
              color="white"
              disabled={this.state.pageNumber === 1}
              onClick={async () => this.navigatePages(-1)}
            >
              &lt; Previous
            </Button>
            <Button
              color="white"
              disabled={this.state.pageNumber >= this.state.pageTotal}
              onClick={async () => this.navigatePages(1)}
            >
              Next &gt;
            </Button>
          </Col>
        </Row>
      </Wrapper>
    )
  }
}

export default SessionTable;