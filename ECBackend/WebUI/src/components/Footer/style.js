import styled from 'styled-components';

import { colors } from 'libs/theme';

const Wrapper = styled.section`
  position: absolute;
  bottom: 0;

  height: 10rem; /* Set the fixed height of the footer here */
  @media (min-width: 992px) {
    height: 3.75rem;
  }
`;

const Item = styled.a`
  color: ${colors.copy} !important;
  text-decoration: none;

  &:hover {
    text-decoration: underline;
  }
  
  &:active {
    text-decoration: underline;
  }
`;

export { Wrapper, Item };