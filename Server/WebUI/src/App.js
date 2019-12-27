import React from 'react';
import 'App.css';

import { BrowserRouter as Router, Route, Switch } from "react-router-dom";

import SignUpPage from "pages/SignUp";
import AdminPage from "pages/Admin";
import ClaimPage from "pages/Claim";
import NormalSignInPage from "pages/NormalSignIn";
import HomePage from 'pages/Home';
import SessionsPage from 'pages/Sessions';
import SettingsPage from 'pages/Settings';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <Router>
          <Switch>
            <Route exact path="/" component={NormalSignInPage}/>
            <Route path="/signUp" component={SignUpPage} />
            <Route path="/signInFor" component={ClaimPage} />
            <Route exact path="/home" component={HomePage} />
            <Route path="/home/sessions" component={SessionsPage} />
            <Route path="/home/settings" component={SettingsPage} />
            <Route path="/admin" component={AdminPage} />
            <Route path="*"> {/* 404 page */}
              <p>Imma 404</p>
            </Route>
          </Switch>
        </Router>
      </header>
    </div>
  );
}

export default App;
