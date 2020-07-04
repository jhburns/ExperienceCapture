import React, { Component } from 'react';

import { Wrapper } from 'components/UserList/style';

import { getData } from 'libs/fetchExtra';

// This component can only be used by admins
class UserList extends Component {
  constructor(props) {
    super(props)

    this.state = {
      users: [],
    };
  }

  async componentDidMount() {
    const url = `/api/v1/users/`;
    const request = await getData(url);

    if (!request.ok) {
      throw new Error(request.status);
    }

    const usersData = await request.json();

    this.setState({
      users: usersData.contentList,
    })
  }

  render() {
    return (
      <Wrapper>
        
      </Wrapper>
    )
  }
}

export default UserList;