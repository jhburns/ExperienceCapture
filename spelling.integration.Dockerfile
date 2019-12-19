FROM node:12.2.0-alpine as build
WORKDIR /app

RUN npm install yaspeller@6.0.2 --global --silent

Copy . . 

RUN yaspeller ./