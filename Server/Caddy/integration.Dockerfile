FROM abiosoft/caddy:1.0.3

ENV aws_domain_name example.com

COPY Caddyfile production.Caddyfile ./

RUN caddy -validate -conf Caddyfile
RUN caddy -validate -conf production.Caddyfile