import os
import json

filename = os.getenv('filename')

path = "./data/processed/{}".format(filename)

with open(path) as f:
	data = json.load(f)
	print("Analyzing data from user: {}".format(data[0]["user"]))

	responceTimes = []

	for d in data:
		if 'UI controller' in d:
			current = d['UI controller']
			if current['responceTime'] not in responceTimes and current['isResponding'] == False:
				responceTimes.append(d['UI controller']['responceTime'])

	responceTimes.remove(0.0) # cleaning up data, initial state is discarded 

	print("Mean reaction time is: {}".format(sum(responceTimes) / len(responceTimes)))