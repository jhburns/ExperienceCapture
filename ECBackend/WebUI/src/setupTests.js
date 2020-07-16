/*eslint no-process-env: "off"*/

import { configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

import { gapi } from 'libs/platform';

configure({ adapter: new Adapter() });

global.gapi = gapi;

// To prevent unhandled promises from being allowed
// From here: https://github.com/facebook/jest/issues/3251#issuecomment-299183885
if (!process.env.LISTENING_TO_UNHANDLED_REJECTION) {
  process.on('unhandledRejection', reason => {
    throw reason;
  })
  // Avoid memory leak by adding too many listeners
  process.env.LISTENING_TO_UNHANDLED_REJECTION = true;
}

process.env.REACT_APP_GOOGLE_CLIENT_ID = "test";
