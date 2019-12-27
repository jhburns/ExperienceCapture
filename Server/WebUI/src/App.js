import React from 'react';
import 'App.css';

import { BrowserRouter as Router, Route, Switch } from "react-router-dom";

import SignUpPage from "pages/SignUp";
import AdminPage from "pages/Admin";
import ClaimPage from "pages/Claim";
import NormalSignInPage from "pages/NormalSignIn";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <Router>
          <Switch>
            <Route exact path="/" component={NormalSignInPage}/>
            <Route path="/signUp" component={SignUpPage} />
            <Route path="/signInFor" component={ClaimPage} />
            {/*<Route path="/home">
              <SignUpPage />
            </Route>
            <Route path="/home/sessions">
              <SignUpPage />
            </Route>
            <Route path="/home/settings">
              <SignUpPage />
            </Route>*/}
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
