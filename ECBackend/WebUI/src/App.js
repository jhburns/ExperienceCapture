import React from 'react';

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
import NotFoundPage from 'pages/NotFound';

import ErrorBoundary from 'components/ErrorBoundary';

import BootstrapProvider from '@bootstrap-styled/provider/lib/BootstrapProvider';

import { verifyEnvironment } from "libs/environment";

import "bootstrap/dist/css/bootstrap.min.css";

import theme from 'libs/theme';

verifyEnvironment(["REACT_APP_GOOGLE_CLIENT_ID"]);

/**
 * The entry point for the application.
 *
 * @returns {object} The application.
 */
function App() {
  return (
    <div className="App">
      <BootstrapProvider theme={theme}>
        <Router>
          <ErrorBoundary>
            <Switch>
              <Route exact path="/" component={NormalSignInPage} />
              <Route exact path="/signUp" component={SignUpPage} />
              <Route exact path="/signInFor" component={ClaimPage} />
              <Route exact path="/home/start" component={HomePage} />
              <Route exact path="/home/sessions" component={SessionsPage} />
              <Route exact path="/home/sessions/id/:id" component={SessionPage} />
              <Route exact path="/home/settings" component={SettingsPage} />
              <Route exact path="/home/sessions/archived" component={ArchivePage} />
              <Route exact path="/admin" component={AdminPage} />
              <Route path="*" component={NotFoundPage} /> {/* 404 page */}
            </Switch>
          </ErrorBoundary>
        </Router>
      </BootstrapProvider>
    </div>
  );
}

export default App;
