filenames <- list.files(path="./data", patter="*.csv", full.names = TRUE)

responce_times <- numeric(0)

for (filename in filenames) {
    csv_file <- read.csv(file = filename)

    for (i in seq(1, length(csv_file$gameObjects.UIController.responceTime))) {
        current_responce_time <- csv_file$gameObjects.UIController.responceTime[i]
        responce_time_numeric <- as.numeric(as.character(current_responce_time))

        is_responding_logical <- as.logical(csv_file$gameObjects.UIController.isResponding[i])

        # I don't know why this is occasionally NA
        check <- !(current_responce_time %in% responce_times) && (is_responding_logical == FALSE)
        if (!is.na(check) && check) {
            responce_times <- c(responce_times, current_responce_time)
        }
    }
}

# Cleaning up data, initial state is discarded 
responce_times_cleaned <- responce_times[2:length(responce_times)]

print(paste(c("Every responce time (sec):", responce_times_cleaned), collapse=" "))
print(paste("Mean reaction time is: ", mean(responce_times_cleaned)))