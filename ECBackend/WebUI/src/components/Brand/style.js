import styled from 'styled-components';

const Image = styled.img`
  width: 2.3rem;
  height: auto;
`;

const Title = styled.a`
  font-size: 1.9rem !important;
  word-wrap: break-word;

  @media (min-width: 992px) {
      font-size: 1.25rem !important;
  }
`;

export { Image, Title };
