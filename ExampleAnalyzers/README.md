# Example Analyzers

The following data analyzers uses some pre-captured data from the Demo Game. The Demo Game is a basic test your reaction type game, play it form more information.

Sample captured data (session ID LRU3) has already been unzipped into the `ExampleAnalyzers/data` folder.

## Formats

Any language can be used to do analysis, but the following are examples are in Python and R.

- Python, JSON format: `ExampleAnalyzers/PythonWithJSON`
- Python, with CSV format: `ExampleAnalyzers/PythonWithCSV`
- Rlang, with CSV format: `ExampleAnalyzers/RlangWithCSV`
- Python, JSON format, on Colaboratory: the following section.

## Analytics Online

Colaboratory is a cool platform for doing analytics that works like Google Docs, but for data. Only the Python, JSON format is available as an example, but the other can easily be adapted for Colaboratory.

How to use:

- [Open the notebook](https://colab.research.google.com/drive/1qRGDUrbBuMuix6RU0Kd4fGJs2J5lowXo).
- File > Save a copy in Drive.
- Press 'play' on the first code block, and select the `LRU3.onlyCaptures.json` locally to upload.
- Press 'play' on the second code block.
