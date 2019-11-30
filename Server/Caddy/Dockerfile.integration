FROM abiosoft/caddy:1.0.3

ENV aws_domain_name example.com

COPY Caddyfile Caddyfile.production ./

RUN caddy -validate -conf Caddyfile
RUN caddy -validate -conf Caddyfile.production