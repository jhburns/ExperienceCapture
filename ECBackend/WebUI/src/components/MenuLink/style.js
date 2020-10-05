import styled from 'styled-components';

import { colors } from 'libs/theme';

const Wrapper = styled.section`

`;

const NavLinkOverride = styled.a`
  color: ${colors.copy} !important;

  font-size: 1.25rem;
  @media (min-width: 992px) {
        font-size: 1rem;
  }

  &:hover {
    border-bottom: 1.5px solid;
    margin-bottom: -1.5px;
    border-color: ${colors.copy};
  }
`;

const Underline = styled.div`
  width: fit-content;
`;

export { Wrapper, NavLinkOverride, Underline };
