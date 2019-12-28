import React, { Component } from 'react';

import Menu from 'components/Menu';
import SignOutButton from 'components/SignOutButton';

import { signOutUser } from 'libs/userManagement';

class SettingsPage extends Component {
  constructor(props) {
    super(props)
    
    this.signOutCallback = this.onSignOut.bind(this);
  }

  async onSignOut() {
    await signOutUser(undefined); // Whether isMock is unknown because its in a different component
    this.props.history.push('/');
  }

  render() {
    return (
      <div>
        <p>Welcome to Settings</p>
        <Menu />
        <SignOutButton onClickCallback={this.signOutCallback} />
      </div>
    );
  }
}

export default SettingsPage;