FROM node:13.1.0-alpine as build
WORKDIR /app

# Needed so that tests always run
# See: https://create-react-app.dev/docs/running-tests/#continuous-integration
ENV CI=true EXTEND_ESLINT=true

COPY package.json package-lock.json /app/
RUN npm ci --silent

COPY jsconfig.json config-overrides.js ./
COPY public ./public
COPY src ./src

RUN npm run lint
RUN npm run test
RUN npm run build

# production environment
FROM abiosoft/caddy:1.0.3
WORKDIR /app/build
COPY --from=build /app/build /app/build

COPY Caddyfile .
RUN caddy -validate -conf Caddyfile

ENTRYPOINT []
CMD ["caddy", "-conf", "Caddyfile"]