FROM node:13.8.0-stretch-slim
WORKDIR /app

# Install packages
COPY package.json package-lock.json ./
RUN npm ci --silent

# Run linting
COPY . .
RUN npm run lint