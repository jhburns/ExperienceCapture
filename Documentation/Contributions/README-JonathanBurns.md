# Jonathan Burn's Contributions 

## Sumbission #2 Planned Features
- Added MongoDB to Docker-compose.
- Added Mongo Driver to Receiver Service.
- Receiver connects to database on startup.
- Docker-compose waits for database to be available before starting Receiver.
- POST /sessions call creates a new session, saves it to the database, and sends that info to the client.
- POST /sessions/{id} checks if the session exists, or sends that data to the session collection.
- GET /sessions/{id} checks if the session exists, and if so returns that data.
- GET /sessions/{id} has query parameters that allow json, or ugly data to be returned optionally.
- DELETE /sessions/{id} checks if the session exists, and changes the session state to closed.

### Additional Unplanned Features
- Continuous Integration with GitHub Actions.
- Minor Client refactoring.
- Standalone development clients for Windows and Mac.
- Docker-compose health-checks.
- Minor documentation refactoring.
- Added linting to Receiver.

## Subission #3 Planned Features
- Added Mongo Driver to Exporter.
- Exporter connects to database on startup.
- Docker-compose waits for database to be available before starting Exporter.
- Exporter prompts user for command, and loops through dialog until quit.
- Exporter sorts session data of user supplied session ids.
- Exporter outputs proccessed session data to file.
- Exporter checks if session exists and is closed before exporting.
- Exporter parses user input when getting session ids from user.

### Additional Unplanned Features
- React app setup to be used as the fronted.
- Caddy reverse-proxy used to connect to the various backend services.
- Docker-compose files seperated based on production and local development.
- Added linting/CI with GitHub Actions to Exporter.
- WebUI production deploy version also added, using Caddy too.
- Linting support added to WebUI, along with CI.