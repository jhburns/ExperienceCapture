FROM gableroux/unity3d:2018.2.11f1
WORKDIR /app

ARG password
ARG username
ARG serial

ENV password=${password} 
ENV username=${username}
ENV serial=${serial}

COPY . .

RUN chmod +x ./build.sh && ./build.sh