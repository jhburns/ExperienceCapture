import { configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

import { gapi } from 'libs/platform';

configure({ adapter: new Adapter() });

global.gapi = gapi;