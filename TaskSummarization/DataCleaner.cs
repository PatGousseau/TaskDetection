using Iveonik.Stemmers;
using Microsoft.ML;
using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Tokenizer; 

namespace TaskSummarization

{
        class DataCleaner
        {

        MLContext context = new MLContext();

        /// <summary>
        /// Cleans data set 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<List<List<string>>> clean(List<List<string>> data)
            {
                List<List<List<string>>> cleanedText = new List<List<List<string>>>();

                foreach (List<string> timeBlock in data)
                {

                    List<List<string>> cleanedTimeBlock = new List<List<string>>();
                    foreach (string windowTitle in timeBlock)
                    {
                        // tokenize, remove stop words, stem and remove non english words
                        List<string> cleanedWindowTitle = removeNonEnglishWordsAndStem(tokenize(windowTitle)); // list of tokens based off the window title
                        cleanedTimeBlock.Add(cleanedWindowTitle);
                        
                    }
                    cleanedText.Add(cleanedTimeBlock);

                }
            test();

            return cleanedText;
            }

        public void test()
        {
            //List<List<string>> list = Queries.GetWindowTitles(60, _date);
            string filePath = @"C:\Users\pcgou\OneDrive\Documents\Work\test.txt";

            //StreamWriter writer = new StreamWriter(filePath);
            //writer.WriteLine("testing");

            File.WriteAllText(filePath, "testingtesting");

            //foreach(List<string> timeBlock in list)
            //{
            //    foreach(string title in timeBlock)
            //    {
            //        await File.WriteAllTextAsync(filePath, title);
            //        File.WriteAllText(filePath, "test");
            //    }
            //}


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
           
                foreach(string word in words)
                {
                    if(hunspell.Spell(word)) // if word is a correct English word
                    {
                       englishWords.Add(stemmer.Stem(word)); // add the stem of the word
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

                
                var emptyData = new List<TextData>();
                var data = context.Data.LoadFromEnumerable(emptyData);

                var tokenization = context.Transforms.Text.TokenizeIntoWords("Tokens", "Text", separators: new[] { ' ', '.', ',', '-','_' })
                    .Append(context.Transforms.Text.RemoveDefaultStopWords("Tokens", "Tokens",
                        Microsoft.ML.Transforms.Text.StopWordsRemovingEstimator.Language.English));

                var stopWordsModel = tokenization.Fit(data);

                var engine = context.Model.CreatePredictionEngine<TextData, TextTokens>(stopWordsModel);

                var newText = engine.Predict(new TextData { Text = text });

                return newText.Tokens;

            }

        /// <summary>
        /// Creates a list of bags of words
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<Dictionary<string, int>> createBagOfWords(List<List<List<string>>> data)
        {

            List<Dictionary<string, int>> bagsOfWords = new List<Dictionary<string, int>>();

            foreach(List<List<string>> timeBlock in data)
            {
                Dictionary<string, int> bag = new Dictionary<string, int>();
                foreach(List<string> windowTitles in timeBlock)
                {
                    foreach(string token in windowTitles)
                    {
                        
                        if(bag.ContainsKey(token))
                        {
                            bag[token] = bag[token] + 1;
                        } else
                        {
                            bag.Add(token, 1);
                        }
                    }
                }
                bagsOfWords.Add(bag);
            }
            return bagsOfWords;
        }

        /// <summary>
        /// Deletes the least occuring words based on topPercentile
        /// </summary>
        /// <param name="bagOfWords"></param>
        /// /// <param name="topPercentile"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, int>> getImportantWords(Dictionary<string,int> bagOfWords, float topPercentile)
        {
            var importantWords = bagOfWords.ToList();

            importantWords.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            int upperBound = (int)(importantWords.Count * (1 - topPercentile));

            importantWords.RemoveRange(0, upperBound);

            return importantWords;
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
