import React, { Component } from 'react';

import Menu from 'components/Menu';
import SessionTable from 'components/SessionTable';

class HomePage extends Component {
  render() {
    return (
      <div>
        <p>Welcome Home</p>
        <Menu />
        <SessionTable 
          sessionsQuery={"createdWithin=1800&isOpen=true"} /* 30 minutes */
          isRenderingDate={false}
        />
      </div>
    );
  }
}

export default HomePage;
