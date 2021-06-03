using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TaskSummarization
{
    class Summarizer
    {


        
        private List<KeyValuePair<BagOfWords,int>> bags; // KeyPair consists of bag of words and task number     
        private string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\GoogleNews-vectors-negative300-SLIM.bin\GoogleNews-vectors-negative300-SLIM.bin";
        private double topPercentile = 0.4; // The top percentage of most frequent words that is kept in the bag of words
        private double similarityThreshold = 0.5; // Minimum cosine similarity for two bags of words to be considered alike
        private int numTasks = 0;
        private List<int> times = new List<int>();
        private int totalTime = 0;
        private int numSecs;
       

        public Summarizer(int numSecs)
        {
            
            this.bags = new List<KeyValuePair<BagOfWords, int>> ();
            this.numSecs = numSecs;
           // times.Add(0);
        }

        public void addBag(BagOfWords newBag) 
        {
            
            // If bag is empty
            if (bags.Count == 0)
            {
                
                numTasks++;
                bags.Add(new KeyValuePair<BagOfWords, int>(newBag, numTasks));
                times.Add(numSecs);
                
            } else
            {
                

                // Collapse if preceding bag is similar to the new one
                if (similarBags(bags[bags.Count - 1].Key,newBag))
                {
                    bags[bags.Count - 1].Key.addWords(newBag);
                    times[times.Count - 1] = times[times.Count - 1] + numSecs;
                    
                } else
                {
                   
                    
                    Boolean isNewTask = true;
                    int newTaskNum = -1; // Task number for newBag

                    // Check if newBag represents the same task as a previous bag
                    foreach (KeyValuePair<BagOfWords,int> bag in bags)
                    {
                        
                        if (similarBags(bag.Key,newBag)) {
                            newTaskNum = bag.Value;
                            isNewTask = false;
                            break;
                        }
                    }
                    if(isNewTask)
                    {
                        numTasks++;
                        newTaskNum = numTasks;
                    }
                    if (bags[bags.Count - 1].Value == newTaskNum)
                    {
                        bags[bags.Count - 1].Key.addWords(newBag);
                        times[times.Count - 1] = times[times.Count - 1] + numSecs;
                    }
                    else
                    {
                        bags.Add(new KeyValuePair<BagOfWords, int>(newBag, newTaskNum));
                        times.Add(numSecs + totalTime);
                    }
                }
            }
            totalTime += numSecs;
        }




        /// <summary>
        /// Returns whether or not the bags are similar
        /// </summary>
        /// <param name="bag1"></param>
        /// <param name="bag2"></param>
        /// <returns></returns>
        private Boolean similarBags(BagOfWords bag1, BagOfWords bag2)
        {
            double[] bag1Vector = getAverageVector(bag1.getBag());
            double[] bag2Vector = getAverageVector(bag2.getBag());

            double cosineSimilarity = 1 - Distance.Cosine(bag1Vector, bag2Vector);
            return cosineSimilarity >= similarityThreshold;

        }


        /// <summary>
        /// Gets the average word2vec vector of a bag of words 
        /// </summary>
        /// <param name="bag"></param>
        /// <param name="vecSize"></param>
        /// <returns></returns>
        private double[] getAverageVector(Dictionary<string, int> bag)
        {
            var vocabulary = new Word2vec.Tools.Word2VecBinaryReader().Read(path);
            int vecSize = vocabulary.VectorDimensionsCount;
            double[] vector = new double[vecSize];
            int numTokens = 0;
            //var words = vocabulary.Words;
           // List<string> toDelete = new List<string>();
            

            foreach (KeyValuePair<string, int> token in bag)
            {
                try
                {
                   // if (vocabulary.ContainsWord(token.Key))
                   // {
                        
                        double[] tokenVector = vocabulary.GetRepresentationFor(token.Key).NumericVector.ToDouble();
                        numTokens++;
                        for (int i = 0; i < vecSize; i++)
                        {
                            vector[i] += tokenVector[i];
                        }
                   // } else
                   // {
                        
                       // bag.Remove(token.Key);
                   // }

                }
                catch
                {
                    // Do nothing for unkown words
                  //  toDelete.Add(token.Key);
                   // Console.WriteLine("                                     no good: " + token.Key);
                }
            }

            // Divide each entry in vector by the number of tokens to get average
            for(int i = 0; i < vecSize; i++)
            {
                vector[i] /= numTokens;
            }

            //for (int j = 0; j < toDelete.Count; j++)
            //{
            //    bag.Remove(toDelete[j]);
            //}

            return vector;

        }

        public List<KeyValuePair<BagOfWords, int>> getBags()
        {
            return this.bags;
        }

        public List<int> getTimes()
        {
            return this.times;
        }

        public void printData()
        {
            int index = 0;
            foreach (KeyValuePair<BagOfWords, int> segment in bags)
            {
                TimeSpan time = TimeSpan.FromSeconds(times[index]);
                string hours = time.ToString(@"hh\:mm\:ss");
                Console.WriteLine("---------------------------- Task: " + segment.Value + ", t= " + hours);
                foreach (KeyValuePair<string, int> token in segment.Key.getBag())
                {
                    Console.WriteLine(token.Key + ": " + token.Value);

                }
                index++;
            }
            Console.ReadLine();
        }



        ///// <summary>
        ///// Collapses both bags of words into one
        ///// </summary>
        ///// <param name="cur"></param>
        ///// <param name="next"></param>
        ///// <returns></returns>
        //private void collapseTasks(BagOfWords curbag, BagOfWords nextBag)
        //{

        //    foreach (KeyValuePair<string, int> token in next)
        //    {
        //        if(cur.ContainsKey(token.Key))
        //        {
        //            cur[token.Key] = cur[token.Key] + next[token.Key];
        //        } else
        //        {
        //            cur.Add(token.Key,token.Value);
        //        }
        //    }
        //    return cur;
        //}

        ///// <summary>
        ///// Compare one time block to the next and collapse into one if similar enough
        ///// </summary>
        ///// <returns></returns>
        //public void compareTaskSegments()
        //{
        //    List<Dictionary<string, int>> allBags = bags.getBags();

        //    int cur = 0; // Index of current bag
        //    int next = 1; // Index of the following bag
        //    int numBags = allBags.Count;
        //    while (next < numBags)
        //    {
        //        if (similarBags(allBags[cur], allBags[next]))
        //        {
        //            // Collapse both bags into one
        //            allBags[cur] = collapseTasks(allBags[cur], allBags[next]);
        //            allBags.RemoveAt(next);
        //            numBags--;
        //        } else
        //        {
        //            cur++;
        //            next++;
        //        }
        //    }
        //}
    }
}
