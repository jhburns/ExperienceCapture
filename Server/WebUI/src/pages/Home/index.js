import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

class HomePage extends Component {
  render() {
    return (
      <div>
        <p>Welcome Home</p>
        <Menu />
        <SessionTable sessionsQuery={"createdWithin=2000"} />
      </div>
    );
  }
}

export default HomePage;
