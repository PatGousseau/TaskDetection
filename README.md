# TaskSummarization

## Overview

 This project uses Natural Language Processing (NLP) and Word2Vec to automatically detect task switches in real-time. Added this project as a feature on 
[Personal Analytics](https://pluto.ifi.uzh.ch/PersonalAnalytics/), a productivity application initiated by the University of Zurich.
This project was completed under the supervision of Dr. Gail C. Murphy, University of British Columbia, and Dr. Thomas Fritz, University of Zurich.


## Description of Algorithm

1. Fetch the window titles used within the last two minutes
2. Reduce noise and create bag of words by tokenizing, stemming, removing stop words and non-English words
3. Remove insignificant words
4. Add it to the list of tasks:
    * If it is similar to the preceding task (using Word2Vec and cosine similarity), collapse into one task
    * Otherwise, create new task and check if this task has been worked on earlier

## Further Improvements

- Consider the duration of the windows used by the user. Adding more importance to those worked on for a longer period of time
- Add a feature that allows the user to choose how many words to include for the description of each tasks

## Possible Applications
    
