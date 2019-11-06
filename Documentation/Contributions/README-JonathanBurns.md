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
