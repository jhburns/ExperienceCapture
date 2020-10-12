import React, { Component } from 'react';

import { getData } from 'libs/fetchExtra';

import SessionRow from 'components/SessionRow';
import OptionSelector from 'components/OptionSelector';
import NotifyBox from 'components/NotifyBox';

import { Row, Col, Button, H2, A, Table, Tbody } from '@bootstrap-styled/v4';
import { Wrapper } from 'components/SessionTable/style';

import { postData, deleteData } from 'libs/fetchExtra';

import queryString from 'query-string';

import { LinkContainer } from 'react-router-bootstrap';

class SessionTable extends Component {
  constructor(props) {
    super(props);

    this.state = {
      sessions: undefined,
      isAllowedToPoll: true,
      pageNumber: 1,
      pageTotal: 0,
      sort: 'newestFirst',
      error: null,
    };

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
        this.setState({ error: Error(archiveRequest.status) });
      }

      await this.updateSessions();
    } catch (err) {
      this.setState({ error: err });
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

    if (!request.ok) {
      this.setState({ error: Error(request.status) });
    }

    const sessionsData = await request.json();
    const sessions = sessionsData.contentList;

    // Removing all the extra data from each session, and flattening
    const sessionsConverted = sessions.map((s) => {
      return {
        id: s.id,
        fullname: s.user.fullname,
        createdAt: parseInt(s.createdAt.$date.$numberLong),
      };
    });

    return {
      sessions: sessionsConverted,
      pageTotal: sessionsData.pageTotal,
      sort: nextSort,
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
      this.setState({ error: new Error("Returned option to Session Table is not valid.") });
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
    if (this.state.error) {
      throw this.state.error;
    }

    const items = [];
    const isEmpty = () => items.length === 0 && this.state.sessions !== undefined;

    if (this.state.sessions !== undefined) {
      for (const [index, value] of this.state.sessions.entries()) {
        items.push(<SessionRow
          key={index}
          sessionData={value}
          buttonData={this.props.buttonData !== undefined ? {
            body: this.props.buttonData.body,
            onClick: this.onTag,
          } : undefined}
        />);
      }
    }

    const options = ["Alphabetically", "Oldest First", "Newest First"];

    return (
      <Wrapper className="mb-5" ref={this.topReference}>
        <Row className="mb-2 mt-3 mt-lg-0">
          <Col xs={12} lg="auto" className="my-auto mb-3">
            <H2 className="d-inline-block m-0 pr-3">
              {this.props.title}
            </H2>
            <br className="d-lg-none" />
            {this.props.link !== undefined &&
              <LinkContainer to={this.props.link.path} data-cy="table-link">
                <A>{this.props.link.name}</A>
              </LinkContainer>
            }
          </Col>
          <Col className="d-flex justify-content-lg-end mt-2 mt-lg-0">
            <OptionSelector
              default={options[2]}
              options={options}
              onClick={this.onSort}
            />
          </Col>
        </Row>
        <Table className="table mb-4">
          <Tbody>
            {items}
          </Tbody>
        </Table>
        {isEmpty() &&
          <Row className="m-0 justify-content-center mb-4 text-center" data-cy="sessions-empty">
            <Col xs={12} lg={6}>
              <NotifyBox className="ml-5 mr-5">{this.props.emptyMessage}</NotifyBox>
            </Col>
          </Row>
        }
        {this.state.sessions !== undefined && <Row>
          <Col className="text-center">
            <Button
              className="mr-2"
              color="white"
              disabled={this.state.pageNumber === 1}
              onClick={async () => this.navigatePages(this.state.pageNumber - 1)}
              data-cy="sessions-previous"
            >
              &lt; Previous
            </Button>
            <Button
              color="white"
              /*
                TODO: Fix this being active even when there are not sessions
                Also fix tests so it doesn't happen again.
              */
              disabled={this.state.pageNumber >= this.state.pageTotal}
              onClick={async () => this.navigatePages(this.state.pageNumber + 1)}
              data-cy="sessions-next"
            >
              Next &gt;
            </Button>
          </Col>
        </Row>}
      </Wrapper>
    );
  }
}

export default SessionTable;