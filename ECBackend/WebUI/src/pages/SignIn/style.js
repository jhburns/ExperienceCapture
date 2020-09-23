import styled from 'styled-components';

const Wrapper = styled.section`

`;

const Illustration = styled.img`
  width: 20rem;
  height: auto;

  @media (min-width: 992px) { 
    width: 35rem;
  };

`;

const PromoTitle = styled.h2`
  margin: 0;
  font-size: 2.8rem;
  line-height: 1.4;
`;

export { Wrapper, Illustration, PromoTitle };