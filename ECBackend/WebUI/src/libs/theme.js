import { makeTheme } from 'bootstrap-styled/lib/theme';


const colors = {
  primary: '#6E2EEE',
  warning: '#D50000',
  background: '#F3F0F5',
  copy: '#161617',
  highlight: ' #FFFFFF',
};

const fonts = 'Rubik, sans-serif';

const theme = makeTheme({
  // Colors
  '$primary': `${colors.primary}`,
  '$warning': `${colors.warning}`,
  '$background': `${colors.background}`,
  '$content': `${colors.copy}`,

  // Base
  '$font-family-base': `${fonts}`,
  '$body-bg': `${colors.background}`,
  '$body-color': `${colors.copy}`,

  // Buttons
  '$btn-primary-bg': `${colors.primary}`,
  '$btn-primary-color': `${colors.background}`,
  '$btn-border-radius': '0.20rem',
  '$btn-danger-bg': `${colors.warning}`,
  '$btn-danger-color': '$white',

  // Links
  '$link-color': `${colors.primary}`,
  '$link-hover-color': `${colors.primary}`,
  '$link-decoration': 'underline',

  // Header
  '$font-size-h1': `1.9rem`,
  '$headings-font-family': `${fonts}`,
  '$headings-font-weight': 500,

  // Modal

});

export { theme, colors };