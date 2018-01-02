# NAudioMicUWPApp

This app takes in microphone input volume levels and outputs it to a progress bar. The app gets the maximum value 100 times a second. Since this app is for a Raspberry Pi 2 that can't handle updating the UI 100 times a second, it spits out the minimum, median, and maximum values for the past 100 values (approximately 1 second).
