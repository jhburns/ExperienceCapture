# base image
FROM node:13.1.0-alpine

# set working directory
WORKDIR /app

# install and cache app dependencies
COPY package.json package-lock.json /app/
RUN npm install --silent

# Saving the arg for later, so both dev and prod can have the same environment
ARG REACT_APP_GOOGLE_CLIENT_ID
ENV REACT_APP_GOOGLE_CLIENT_ID=$REACT_APP_GOOGLE_CLIENT_ID

# start app
CMD ["npm", "start"]