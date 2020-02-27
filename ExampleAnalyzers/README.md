# Example Analyzers

The following data analyzers uses some pre-captured data from the Demo Game. The Demo Game is a basic test your reaction type game, play it form more information.

Sample captured data (session ID FHEB) has already been unzipped into the `ExampleAnalyzers/data` folder.

## Formats

Any language can be used to do analysis, but the examples in Python and R.

- Python, JSON format: `ExampleAnalyzers/PythonWithJSON`
- Python, with CSV format: `ExampleAnalyzers/PythonWithCSV`
- Rlang, with CSV format: `ExampleAnalyzers/RlangWithCSV`
- Python, JSON format, on Colaboratory: the following section.

## Analytics Online

[comment]: <> (Source: https://research.google.com/colaboratory/faq.html)

Colaboratory is a free platform for doing analytics that works like Google Docs, but for data. Only the Python, JSON format is available as an example, but any Python code can be adapted for Colaboratory. Rlang/Other languages are not supported yet.

How to use:

- [Open the notebook](https://colab.research.google.com/drive/1qRGDUrbBuMuix6RU0Kd4fGJs2J5lowXo).
- File > Save a copy in Drive.
- Press 'play' on the first code block, and select the `FHEB.onlyCaptures.json` locally to upload.
- Press 'play' on the second code block.

[comment]: <> (TODO: Update to latest exporter version)