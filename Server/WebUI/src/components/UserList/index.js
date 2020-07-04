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
    console.log(usersData);

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
        <Col>
          <p>{value.fullname}</p>
          <Button onClick={() => this.onDelete(value.id)} >Delete</Button>
        </Col>
      </Row>);
    }

    return (
      <Wrapper>
        {items}
      </Wrapper>
    )
  }
}

export default UserList;