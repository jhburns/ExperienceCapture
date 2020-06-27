import React, { Component } from 'react';

import { getData } from 'libs/fetchExtra';

import SessionRow from 'components/SessionRow';
import Dropdown from 'components/Dropdown';

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
      sort: 'newestFirst'
    }

    this.onTag = this.onTag.bind(this);
    this.getSessions = this.getSessions.bind(this);
    this.updateSessions = this.updateSessions.bind(this);
    this.navigatePages = this.navigatePages.bind(this);
    this.onSort = this.onSort.bind(this);
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

      await this.updateSessions();
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

  async getSessions(page, sort = null) {
    const nextSort = sort ?? this.state.sort;

    let queryOptions = this.props.queryOptions;
    queryOptions.ugly = true;
    queryOptions.page = page;
    queryOptions.sort = nextSort;
    const query = queryString.stringify(queryOptions);

    const url = `/api/v1/sessions?${query}`;
    const request = await getData(url);
    // TODO: add status code checks to this request
    const sessionsData = await request.json();
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
      sort: nextSort
    };
  }

  async navigatePages(nextPage, sort = null) {
    const sessions = await this.getSessions(nextPage, sort);

    this.topReference.current.scrollIntoView();

    this.setState(Object.assign(sessions, {
      pageNumber: nextPage,
    }));
  }

  async onSort(option) {
    let mappedOption = null;

    switch (option) {
      case "Alphabetically":
        mappedOption = "alphabetical";
        break;
      case "Oldest First":
        mappedOption = "oldestFirst";
        break;
      case "Newest First":
        mappedOption = "newestFirst";
        break;
      default:
        throw new Error("Returned option to Session Table is not valid.");
    }

    await this.navigatePages(1, mappedOption);
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
        <h2 className="mb-3 pl-3 pl-lg-0">
          {this.props.title}
        </h2>
        <Row className="mb-2">
          <Col>
            <Dropdown
              title="Sort By"
              options={["Alphabetically", "Oldest First", "Newest First"]}
              onClick={this.onSort}
            />
          </Col>
        </Row>
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
              onClick={async () => this.navigatePages(this.state.pageNumber - 1)}
            >
              &lt; Previous
            </Button>
            <Button
              color="white"
              disabled={this.state.pageNumber >= this.state.pageTotal}
              onClick={async () => this.navigatePages(this.state.pageNumber + 1)}
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