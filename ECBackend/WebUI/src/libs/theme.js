import { makeTheme } from 'bootstrap-styled/lib/theme';

const primary = '#6E2EEE';
const warning = '#D50000';
const background = '#F3F0F5';
const content = '#161617';

const theme = makeTheme({
  // Colors
  '$primary': `${primary}`,
  '$warning': `${warning}`,
  '$background': `${background}`,
  '$content': `${content}`,

  // Base
  '$font-family-base': 'Rubik, sans-serif',
  '$body-bg': `${background}`,
  '$body-color': `${content}`,

  // Buttons
  '$btn-primary-bg': `${primary}`,
  '$btn-primary-color': `${background}`,
  '$btn-border-radius': '0.20rem',
  '$btn-danger-bg': `${warning}`,
  '$btn-danger-color': '$white',

  // Links
  '$link-color': `${primary}`,
  '$link-decoration': 'underline',
});

export default theme;