import styled from 'styled-components';

import { colors } from 'libs/theme';

const Wrapper = styled.section`

`;

const NavLinkOverride = styled.a`
  color: ${colors.copy} !important;
  &:hover {
    border-bottom: 1.5px solid;
    margin-bottom: -1.5px;
    border-color: ${colors.copy};
  }
`;

export { Wrapper, NavLinkOverride };
