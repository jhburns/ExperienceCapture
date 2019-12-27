import React, { Component } from 'react';
import { Link } from "react-router-dom";

class HomeButton extends Component {
  render() {
    return (
      <Link to="/home">
	      <button onClick={this.props.onClickCallback}>Go Home</button>
      </Link>
	  )
  }
}

export default HomeButton;