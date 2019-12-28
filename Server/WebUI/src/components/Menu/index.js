import React, { Component } from 'react';

import { Link }from 'react-router-dom';

class Menu extends Component {
  render() {
    return (
      <div>
        <Link to="/home">Home</Link>
        <Link to="/home/sessions">Sessions</Link>
        <Link to="/home/settings">Settings</Link>
      </div>
    )
  }
}
export default Menu;