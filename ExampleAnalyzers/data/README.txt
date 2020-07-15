README - About file layout

- EGN4.onlyCaptures.json: Only data collected from the GetCapture() method.
- EGN4.sessionInfo.json: Extra info about the session, from the client.
- CSVs/ : all of the GetCapture() data, with one file per scene converted to a CSV format.
- extras/EGN4.database.json: Extra info about the session, from the server.
- extras/EGN4.raw.json: A straight dump of the session information. Only really useful in the case export didn't work correctly.

General documentation: https://github.com/jhburns/ExperienceCapture/tree/master/Documentation#documentation
More info about the export format: https://github.com/jhburns/ExperienceCapture/blob/master/Documentation/Export-Format.md