FROM node:13.1.0-alpine as build
WORKDIR /app
ENV CI=true

COPY package.json package-lock.json /app/
RUN npm ci --silent

ARG REACT_APP_GOOGLE_CLIENT_ID

COPY jsconfig.json config-overrides.js ./
COPY public ./public
COPY src ./src

RUN npm run build

# production environment
FROM abiosoft/caddy:1.0.3
WORKDIR /app/build
COPY --from=build /app/build /app/build

COPY Caddyfile .

ENTRYPOINT []
CMD ["caddy", "-conf", "Caddyfile"]