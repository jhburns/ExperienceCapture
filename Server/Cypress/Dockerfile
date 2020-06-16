FROM cypress/browsers:node13.8.0-chrome81-ff75

WORKDIR /app

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.6.0/wait /wait
RUN chmod +x /wait

# Install packages
COPY package.json package-lock.json ./
RUN npm ci --silent

# Run tests
COPY . .
RUN cp cypress.integration.json cypress.json

CMD ["npm", "run", "start", "--", "--browser", "chrome"]