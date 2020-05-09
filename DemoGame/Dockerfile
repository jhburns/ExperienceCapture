FROM gableroux/unity3d:2018.2.11f1
WORKDIR /app

COPY . .

RUN chmod +x ./build.sh && ./build.sh || exit 1