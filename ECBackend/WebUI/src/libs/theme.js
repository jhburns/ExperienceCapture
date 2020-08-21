import { makeTheme } from 'bootstrap-styled/lib/theme';

const theme = makeTheme({
  // Colors
  '$primary': '#6E2EEE',
  '$warning': '#D50000',

  // Base
  '$font-family-base': 'Rubik, sans-serif',
  'body-bg': '#F3F0F5',
  'body-color': '#161617',

  // Buttons
  '$btn-primary-bg': '#6E2EEE',
  '$btn-primary-color': '#F3F0F5',
  '$btn-border-radius': '0.20rem',
  '$btn-danger-bg': '#D50000',
  '$btn-danger-color': '#FFF',

  // Links
  '$link-color': '#6E2EEE',
  '$link-decoration': 'underline',
});

export default theme;