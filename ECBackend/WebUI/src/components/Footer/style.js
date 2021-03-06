import styled from 'styled-components';

import { colors } from 'libs/theme';

const Wrapper = styled.section`
  position: absolute;
  bottom: 0;
  width: 100%;

  height: 7rem; /* Set the fixed height of the footer here */
  @media (min-width: 992px) {
    height: 3rem;
  }
`;

const Item = styled.a`
  color: ${colors.copy} !important;
  font-size: 1.5rem;
  line-height: 1.1;

  text-decoration: none;
  &:hover {
    text-decoration: underline;
  }
  
  &:active {
    text-decoration: underline;
  }
`;

export { Wrapper, Item };