import os
import json

about_filename = "./data/QN4F.sessionInfo.json"
captures_filename = "./data/QN4F.onlyCaptures.json"

with open(about_filename) as f:
    data = json.load(f)
    print("Analyzing data from user: {}".format(data[0]["playerName"]))
    print("---")

with open(captures_filename) as f:
	data = json.load(f)

	responce_times = []

    # Looks for unique keys of responceTime key
	for d in data:
		current = d["gameObjects"]["UIController"]
		if current["responceTime"] not in responce_times and current["isResponding"] == False:
			responce_times.append(current["responceTime"])

	responce_times.remove(0.0) # cleaning up data, initial state is discarded 

	print("Every responce time (sec): {}".format(responce_times))
	print("Mean reaction time is: {}".format(sum(responce_times) / len(responce_times))) 