FROM node:12.2.0-alpine as build
WORKDIR /app
ARG REACT_APP_GOOGLE_CLIENT_ID
ENV PATH /app/node_modules/.bin:$PATH

COPY package.json package-lock.json /app/
RUN npm install --silent

COPY jsconfig.json .
COPY public ./public
COPY src ./src

RUN npm run lint
RUN npm run build

# production environment
FROM abiosoft/caddy:1.0.3
WORKDIR /app/build
COPY --from=build /app/build /app/build

COPY Caddyfile .
RUN caddy -validate -conf Caddyfile

ENTRYPOINT []
CMD ["caddy", "-conf", "Caddyfile"]