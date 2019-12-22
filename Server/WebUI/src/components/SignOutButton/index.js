import React, { Component } from 'react';

class SignOutButton extends Component {
  render() {
    return (
		  <button onClick={this.props.onClickCallback}>Sign Out</button>
	  )
  }
}
export default SignOutButton;