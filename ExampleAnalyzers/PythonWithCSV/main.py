import os
import csv
from decimal import Decimal

# Find all files that end with *.csv
csv_filepaths = []
for file in os.listdir("./data"):
    if file.endswith(".csv"):
        csv_filepaths.append(os.path.join("./data", file))

responce_times = []
rt = "gameObjects.UIController.responceTime"
ir = "gameObjects.UIController.isResponding"

# Looks for unique keys of responceTime key
for filepath in csv_filepaths:
	with open(filepath) as csvfile:
		reader = csv.DictReader(csvfile)

		for row in reader:
			responce_time = Decimal(row[rt])

			if responce_time not in responce_times and row[ir] == "False":
				responce_times.append(responce_time)

responce_times.remove(Decimal(0.0)) # cleaning up data, initial state is discarded 

responce_times_str = list(map(str, responce_times)) # Change decimals to strings so they print correctly

print("Every responce time (sec): {}".format(responce_times_str))
print("Mean reaction time is: {}".format(float(sum(responce_times) / len(responce_times)))) 