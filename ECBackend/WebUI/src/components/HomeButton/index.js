import React, { Component } from 'react';
import { Link } from "react-router-dom";

import { Wrapper } from 'components/HomeButton/style';

class HomeButton extends Component {
  render() {
    return (
      <Wrapper>
        <Link to="/home/start" className="btn btn-dark btn-block" data-cy="go-home">
          Go Home
        </Link>
      </Wrapper>
	  )
  }
}

export default HomeButton;