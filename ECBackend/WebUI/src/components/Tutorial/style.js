import styled from 'styled-components';

import { colors } from 'libs/theme';

const Wrapper = styled.section`

`;

const Info = styled.section`
  background-color: ${colors.highlight};
  padding: 1rem 1rem 1rem 1rem;
`;

const Item = styled.li`
    padding-top: 0.5rem;
`;

export { Wrapper, Info, Item };