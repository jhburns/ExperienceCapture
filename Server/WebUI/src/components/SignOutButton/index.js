import React, { Component } from 'react';

import { Button } from 'components/SignOutButton/style';

class SignOutButton extends Component {
  render() {
    return (
      <Button 
        onClick={this.props.onClickCallback}
        className="btn btn-outline-dark btn-block"
        data-cy="sign-out"
      >
        Sign Out
      </Button>
	  )
  }
}
export default SignOutButton;