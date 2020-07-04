import React, { Component } from 'react';

import { Wrapper } from 'components/UserList/style';

import { getData, deleteData, } from 'libs/fetchExtra';
import { Row, Col, Button, } from '@bootstrap-styled/v4';

// This component can only be used by admins
class UserList extends Component {
  constructor(props) {
    super(props)

    this.state = {
      users: [],
    };

    this.onDelete = this.onDelete.bind(this);
    this.getUsers = this.getUsers.bind(this);
  }

  async onDelete(id) {
    const url = `/api/v1/users/${id}`;
    const request = await deleteData(url);

    if (!request.ok) {
      throw new Error(request.status);
    }

    await this.getUsers();
  }

  async getUsers() {
    const url = `/api/v1/users/`;
    const request = await getData(url);

    if (!request.ok) {
      throw new Error(request.status);
    }

    const usersData = await request.json();

    this.setState({
      users: usersData.contentList,
    });
  }

  async componentDidMount() {
    await this.getUsers();
  }

  render() {
    const items = [];

    for (const [index, value] of this.state.users.entries()) {
      items.push(<Row key={index}>
        <Col className="d-inline-block">
          <p className="d-inline-block mr-3">{value.fullname}</p>
          <button
            className="btn btn-outline-dark"
            onClick={() => this.onDelete(value.id)}
          >
            Delete
          </button>
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
    )
  }
}

export default UserList;