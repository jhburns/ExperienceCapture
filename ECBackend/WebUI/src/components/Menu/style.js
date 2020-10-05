import styled from 'styled-components';

import { colors } from 'libs/theme';

const Wrapper = styled.section`

`;

const Background = styled.section`
  background-color: ${colors.highlight};
`;

const Toggle = styled.button`
  outline-width: ${props => props.isOpen ? '1px' : '0'};
  outline-color: ${colors.primary};
`;

const Hamburger = styled.img`
  width: 3.25rem;
  height: auto;

  margin-top: ${props => props.isOpen ? '3px' : '0px'};
`;

export { Wrapper, Background, Toggle, Hamburger };
