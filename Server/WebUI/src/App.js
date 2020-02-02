import React from 'react';
import 'App.css';

import { BrowserRouter as Router, Route, Switch, } from "react-router-dom";

import SignUpPage from "pages/SignUp";
import AdminPage from "pages/Admin";
import ClaimPage from "pages/Claim";
import NormalSignInPage from "pages/NormalSignIn";
import HomePage from 'pages/Home';
import SessionsPage from 'pages/Sessions';
import SettingsPage from 'pages/Settings';
import ArchivePage from 'pages/ArchivedSessions';
import SessionPage from 'pages/Session';

import BootstrapProvider from '@bootstrap-styled/provider/lib/BootstrapProvider';

import "bootstrap/dist/css/bootstrap.min.css";

const theme = {
  '$font-family-base': 'sans-serif',
  '$body-bg': '#FFFFFF',
  '$body-color': '#000000',
  //'$btn-primary-bg': '#000000',
  //'$btn-border-radius': '.035rem',
  //'$link-color': '#000000',
};

function App() {
  return (
    <div className="App">
      <BootstrapProvider theme={theme}>
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
      </BootstrapProvider>
    </div>
  );
}

export default App;
