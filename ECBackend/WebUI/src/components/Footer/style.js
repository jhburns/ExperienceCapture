import styled from 'styled-components';

const Wrapper = styled.section`
  position: absolute;
  bottom: 0;
  width: 80%;
  height: 60px; /* Set the fixed height of the footer here */
  line-height: 60px; /* Vertically center the text there */
`;

const Item = styled.a`
  color: #7a7b88 !important;
  text-decoration: underline;

  &:hover {
    color: #000000b3 !important;
  }
  
  &:active {
    color: #00000080 !important;
  }
`;

export { Wrapper, Item, };