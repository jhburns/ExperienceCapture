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
import ArchivePage from 'pages/ArchivedSessions';
import SessionPage from 'pages/Session';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <Router>
          <Switch>
            <Route exact path="/" component={NormalSignInPage}/>
            <Route exact path="/signUp" component={SignUpPage} />
            <Route exact path="/signInFor" component={ClaimPage} />
            <Route exact path="/home" component={HomePage} />
            <Route exact path="/home/sessions" component={SessionsPage} />
            <Route exact path="/home/sessions/:id" component={SessionPage} />
            <Route exact path="/home/settings" component={SettingsPage} />
            <Route exact path="/home/archived" component={ArchivePage} />
            <Route exact path="/admin" component={AdminPage} />
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
