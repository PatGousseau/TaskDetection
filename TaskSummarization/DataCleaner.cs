using Iveonik.Stemmers;
using Microsoft.ML;
using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Tokenizer; 

namespace TaskSummarization

{
        class DataCleaner
        {

            /// <summary>
            /// Cleans data set 
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public List<List<string>> clean(List<string> data)
            {
                List<List<string>> cleanedText = new List<List<string>>();

                foreach (string timeBlock in data)
                {
                    // tokenize, remove stop words, stem and remove non english words
                    List<string> cleanedTimeBlock = removeNonEnglishWordsAndStem(tokenize(timeBlock));
                    cleanedText.Add(cleanedTimeBlock);
                }
                return cleanedText;
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

                var context = new MLContext();
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
