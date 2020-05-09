# This check is special because it needs to check files from another subproject
# It copies the client folder from the ClientDevelopmentGame

# Run with `env DOCKER_BUILDKIT=1 docker build ../ -f Dockerfile` from this folder

FROM gableroux/unity3d:2018.2.11f1
WORKDIR /app

ARG password
ARG username
ARG serial

ENV password ${password} 
ENV username ${username}
ENV serial ${serial}

COPY SetupTestGame/ .
COPY ./ClientDevelopmentGame/Assets/ExperienceCapture/ ./Assets/ExperienceCapture

RUN chmod +x ./build.sh && ./build.sh