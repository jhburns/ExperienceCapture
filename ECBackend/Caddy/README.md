[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Caddy/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Caddy%22)

# Caddy

The back-end used [Caddy 1.0](https://caddyserver.com/v1/) as a reserve proxy for each service.

[Check the documentation for more.](https://caddyserver.com/v1/docs)

## About

There are two different configurations used, one for local development, `Caddyfile`,
and one for production, `Caddyfile.production`. Routing should behave the same between
both, with the main difference being SSL is disabled for local development.

[comment]: <> (TODO: add rate-limiting, only in production, this should work: https://hub.docker.com/r/xuqingfeng/caddy-rate-limit)