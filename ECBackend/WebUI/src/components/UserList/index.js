import React, { Component } from 'react';

import { Wrapper } from 'components/UserList/style';

import { getData, deleteData } from 'libs/fetchExtra';
import { Row, Col, Button, P } from '@bootstrap-styled/v4';

// This component can only be used by admins
class UserList extends Component {
  constructor(props) {
    super(props);

    this.state = {
      users: [],
      error: null,
    };

    this.onDelete = this.onDelete.bind(this);
    this.getUsers = this.getUsers.bind(this);
  }

  async onDelete(id) {
    try {
      const url = `/api/v1/users/${id}`;
      const request = await deleteData(url);

      if (!request.ok) {
        this.setState({ error: new Error(request.status) });
      }

      await this.getUsers();
    } catch (err) {
      this.setState({ error: err });
    }
  }

  async getUsers() {
    const url = `/api/v1/users/`;
    const request = await getData(url);

    if (this.isCurrentlyMounted)
    {
      if (!request.ok) {
        this.setState({ error: new Error(request.status) });
      }
    }

    const usersData = await request.json();

    if (this.isCurrentlyMounted) {
      this.setState({
        users: usersData.contentList,
      });
    }
  }

  async componentDidMount() {
    // Setups a check, because component may
    // Be re-rendered before fetch is complete
    this.isCurrentlyMounted = true;

    await this.getUsers();
  }

  componentWillUnmount() {
    this.isCurrentlyMounted = false;
  }

  render() {
    if (this.state.error) {
      throw this.state.error;
    }

    const items = [];

    for (const [index, value] of this.state.users.entries()) {
      items.push(<Row key={index}>
        <Col className="d-inline-block">
          <P className="d-inline-block mr-3">{value.fullname}</P>
          <Button
            onClick={() => this.onDelete(value.id)}
            data-cy="delete-others"
            color="warning"
          >
            Delete
          </Button>
        </Col>
      </Row>);
    }

    return (
      <Wrapper className="text-center mt-5">
        <Row className="mb-3">
          <Col>
            <h3>Admin Controls</h3>
          </Col>
        </Row>
        {items}
      </Wrapper>
    );
  }
}

export default UserList;