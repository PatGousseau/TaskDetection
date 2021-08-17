# Test Notes

The ground truth data is uploaded here: https://drive.google.com/file/d/1I5FNNeAdqIW5uXKiIVY5KgqDN6KY89bQ/view?usp=sharing

## Description of each type of tests

### Horizontal Test

This is a measure of how accurately the program can detect task switches. It checks how often the program correctly detects a task switch within a 5 minute window.
For example if the user switched tasks at 9:44:00, the program correctly identified a task switch if it finds one anywhere between 9:41:30 and 9:46:30.

### Vertical Test

This is a measure of the similarity between the content of each ground truth task to the found task. More specifically, for each ground truth task segment, it compares the content of
the found task that corresponds with the time of that ground truth segment. This is accomplished by comparing the description of the ground truth task segment to the found task's
bag of words.

### Task Association Test

This is a measure of how accurately the program can detect that a task has been repeated. For instance, it should be able to determine that from 9:44 - 10:14, the user worked on
task X, and from 2:13 - 2:22, the user worked on task X again. 
