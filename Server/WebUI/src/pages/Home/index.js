import React, { Component } from 'react';

import { Link } from 'react-router-dom';

class HomePage extends Component {
  render() {
    return (
      <div>
        <p>Welcome Home</p>
        <Link to="/home/sessions">Sessions</Link>
        <Link to="/home/settings">Settings</Link>
      </div>
    );
  }
}

export default HomePage;
