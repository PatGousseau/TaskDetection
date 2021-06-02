using Iveonik.Stemmers;
using Microsoft.ML;
using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TaskSummarization

{
    class BagOfWords
    {

        private Dictionary<string, int> bag;
        private double topPercentile;

        public BagOfWords(List<string> windowTitles, double topPercentile)
        {
            this.topPercentile = topPercentile;
            createBagOfWords(windowTitles);

        }



        /// <summary>
        /// Cleans data set 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<List<string>> clean(List<string> data)
        {
            List<List<string>> cleanedText = new List<List<string>>(); // List of lists of tokens

            foreach (string windowTitle in data)
            {
                // Tokenize, remove stop words, stem and remove non english words
                List<string> cleanedWindowTitle = removeNonEnglishWordsAndStem(tokenize(windowTitle)); // List of tokens based off the window title
                cleanedText.Add(cleanedWindowTitle);

            }

            return cleanedText;
        }

        /// <summary>
        /// Addes a bag of words to this
        /// </summary>
        /// <param name="newBag"></param>
        /// <returns></returns>
        public void addWords(BagOfWords newBag)
        {

            Dictionary<string, int> toAdd = newBag.getBag();
            foreach (KeyValuePair<string, int> token in toAdd)
            {
                if (bag.ContainsKey(token.Key))
                {
                    bag[token.Key] = bag[token.Key] + toAdd[token.Key];
                }
                else
                {
                    bag.Add(token.Key, token.Value);
                }
            }
            removeInfrequentWords();
        }



        /// <summary>
        /// Removes non English words and stems each word
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private List<string> removeNonEnglishWordsAndStem(string[] words)
        {
            EnglishStemmer stemmer = new EnglishStemmer();
            Hunspell hunspell = new Hunspell("en_us.aff", "en_us.dic");
            List<string> englishWords = new List<string>();

            foreach (string word in words)
            {
                if (!Regex.IsMatch(word.ToLower(), "google") && !Regex.IsMatch(word.ToLower(), "search")) // remove google search
                {
                    if (hunspell.Spell(word) && !Regex.IsMatch(word, @"^\d+$")) // If word is a correct English word
                    {
                        englishWords.Add(stemmer.Stem(word)); // Add the stem of the word
                    }
                }
            }
            return englishWords;
        }

        /// <summary>
        /// Tokenizes and removes all stop words
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string[] tokenize(string text)
        {

            MLContext context = new MLContext();
            var emptyData = new List<TextData>();
            var data = context.Data.LoadFromEnumerable(emptyData);

            var tokenization = context.Transforms.Text.TokenizeIntoWords("Tokens", "Text", separators: new[] { ' ', ',', '-', '_' })
                .Append(context.Transforms.Text.RemoveDefaultStopWords("Tokens", "Tokens",
                    Microsoft.ML.Transforms.Text.StopWordsRemovingEstimator.Language.English));

            var stopWordsModel = tokenization.Fit(data);

            var engine = context.Model.CreatePredictionEngine<TextData, TextTokens>(stopWordsModel);

            var newText = engine.Predict(new TextData { Text = text });

            return newText.Tokens;

        }

        /// <summary>
        /// Creates bag of words
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void createBagOfWords(List<string> data)
        {

            List<List<string>> cleanedData = clean(data);
            this.bag = new Dictionary<string, int>();

            foreach (List<string> windowTitle in cleanedData)
            {
                foreach (string token in windowTitle)
                {

                    if (bag.ContainsKey(token))
                    {
                        bag[token] = bag[token] + 1;
                    }
                    else
                    {
                        bag.Add(token, 1);
                    }
                }
            }
            removeInfrequentWords();
        }

        /// <summary>
        /// Deletes the least occuring words based on topPercentile
        /// </summary>
        /// <param name="bagOfWords"></param>>
        /// <returns></returns>
        public void removeInfrequentWords()
        {
            var importantWords = bag.ToList();

            importantWords.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            int upperBound = (int)(importantWords.Count * (1 - topPercentile));

            importantWords.RemoveRange(0, upperBound);
            this.bag = importantWords.ToDictionary(x => x.Key, x => x.Value);

        }

        public Dictionary<string, int> getBag()
        {
            return this.bag;
        }


        private class TextData
        {
            public string Text { get; set; }
        }

        private class TextTokens : TextData
        {
            public string[] Tokens { get; set; }
        }
    }
}
