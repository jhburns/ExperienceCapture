FROM cypress/browsers:node13.8.0-chrome81-ff75
WORKDIR /app

# Add wait
ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.6.0/wait /wait

# Add Docker Compose
RUN curl -L "https://github.com/docker/compose/releases/download/1.25.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

# Make both executable
RUN chmod +x /wait /usr/local/bin/docker-compose

# Install packages
COPY package.json package-lock.json ./
RUN npm ci --silent

# Run tests
COPY . .

# Override the local cypress config file
RUN cp cypress.integration.json cypress.json

CMD ["npm", "run", "start", "--", "--browser", "chrome"]