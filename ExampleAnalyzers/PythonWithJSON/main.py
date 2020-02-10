import os
import json

aboutFilename = "./data/LRU3.sessionInfo.json"
capturesFilename = "./data/LRU3.onlyCaptures.json"

with open(aboutFilename) as f:
    data = json.load(f)
    print("Analyzing data from user: {}".format(data[0]["playerName"]))
    print("---")

with open(capturesFilename) as f:
	data = json.load(f)

	responceTimes = []

    # Looks for unique keys of responceTime key
	for d in data:
		current = d["gameObjects"]["UIController"]
		if current["responceTime"] not in responceTimes and current["isResponding"] == False:
			responceTimes.append(current["responceTime"])

	responceTimes.remove(0.0) # cleaning up data, initial state is discarded 

	print("Every responce time (sec): {}".format(responceTimes))
	print("Mean reaction time is: {}".format(sum(responceTimes) / len(responceTimes))) 