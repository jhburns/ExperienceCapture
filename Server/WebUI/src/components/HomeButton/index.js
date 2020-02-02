import React, { Component } from 'react';
import { Link } from "react-router-dom";

class HomeButton extends Component {
  render() {
    return (
      <Link to="/home" className="btn btn-dark btn-block">
        Go Home
      </Link>
	  )
  }
}

export default HomeButton;