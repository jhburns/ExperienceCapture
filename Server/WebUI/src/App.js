import React from 'react';
import logo from './logo.svg';
import './App.css';

import SignIn  from "components/SignIn";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
		<SignIn clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID} />
      </header>
    </div>
  );
};

export default App;
