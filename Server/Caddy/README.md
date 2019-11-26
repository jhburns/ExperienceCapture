[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/API%20Integration/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Receiver+Integration%22)

# Caddy

The backend used [Caddy 1.0](https://caddyserver.com/v1/) as a reserve proxy for each service.

See: https://caddyserver.com/v1/docs for documentation. 

## About

There are two different configurations used, one for local development (*Caddyfile*)
and one for production (*Caddyfile.production*). Routing should behave the same between
both, with the main difference being SSL setup as it isn't possible for local development. 