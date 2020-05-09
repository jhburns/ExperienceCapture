FROM gableroux/unity3d:2018.2.11f1
WORKDIR /app

ARG password
ARG username
ARG serial
ARG test

ENV password ${password} 
ENV username ${username}
ENV serial ${serial}
ENV test ${test}

RUN echo "$test"

COPY . .

RUN chmod +x ./build.sh && ./build.sh