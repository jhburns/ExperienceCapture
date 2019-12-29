import React, { Component } from 'react';

import Menu from 'components/Menu';
import SignOutButton from 'components/SignOutButton';
import GetSignUpLink from 'components/GetSignUpLink';

import { signOutUser } from 'libs/userManagement';

class SettingsPage extends Component {
  constructor(props) {
    super(props)
    
    this.signOutCallback = this.onSignOut.bind(this);
  }

  async onSignOut() {
    await signOutUser(undefined); // Whether this is mock is unknown because that state is in a different component
    this.props.history.push('/');
  }

  render() {
    return (
      <div>
        <p>Welcome to Settings</p>
        <Menu />
        <SignOutButton onClickCallback={this.signOutCallback} />
        <GetSignUpLink />
      </div>
    );
  }
}

export default SettingsPage;