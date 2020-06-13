FROM cypress/browsers:node13.8.0-chrome81-ff75

WORKDIR /app

# Install packages
COPY package.json package-lock.json ./
RUN npm ci --silent

# Run tests
COPY . .
RUN npm run start -- --browser chrome