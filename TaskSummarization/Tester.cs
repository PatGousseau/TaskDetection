using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSummarization
{
    class Tester
    {

        private int numSecs;
        private Summarizer summarizer;
        public Tester(int numSecs)
        {
            this.numSecs = numSecs;

            //init();
        }

        private void init()
        {
            double topPercentile = 0.4;

            List<List<string>> data = getInputData(numSecs);
            this.summarizer = new Summarizer(numSecs);

            foreach (List<string> titles in data)
            {
                BagOfWords bag = new BagOfWords(titles, topPercentile);
                summarizer.addBag(bag);

            }
        }

        public float compare()
        {
            int correctPoints = 0;
            int totalPoints = 0;
            int ans = 0;

            List<int> times = summarizer.getTimes();
            List<KeyValuePair<BagOfWords, int>> bags = summarizer.getBags();
            List<KeyValuePair<int[], int>> correctData = getCorrectData();
            int curTime = 0; // Current time
            

            for(int i = 0; i < times.Count; i++)
            {
                int upperBound = times[i]; // End time of the current task

                while((curTime + numSecs) <= upperBound)
                {
                    int correctTaskNum = getTaskNumber(curTime, curTime + numSecs, correctData);

                    if (bags[i].Value == correctTaskNum)
                    {
                        correctPoints++;
                    }
                    
                                                                     
                    curTime += numSecs; // Go to next time block
                    
                    if(correctTaskNum > 0) // If it is not a transition period
                    {
                        totalPoints++;
                    }
                    
                }
            }

            ans = correctPoints / totalPoints;

            return ans;
        }

        private int getTaskNumber(int startTime, int endTime, List<KeyValuePair<int[], int>> correctData)
        {

            for(int i = 0; i < correctData.Count; i++)
            {
                if(startTime >= correctData[i].Key[0] && endTime <= correctData[i].Key[1])
                {
                    return correctData[i].Value;
                }
            }
            return -1; // transition period



        }









        public List<KeyValuePair<int[], int>> getCorrectData()
        {
            string format = "g";
            var culture = CultureInfo.InvariantCulture;
            List<KeyValuePair<int[], int>> truth = new List<KeyValuePair<int[], int>>();
            string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\window_titles\P04\truth.txt";
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 1; i < lines.Length; i++)
                {

                    string[] items = lines[i].Split(',');

                    int[] time = new int[2]; // time[0] = start time, time[1] = end time 
                    TimeSpan start;
                    TimeSpan end;
                    TimeSpan.TryParseExact(items[0], format, culture, TimeSpanStyles.AssumeNegative, out start);
                    TimeSpan.TryParseExact(items[1], format, culture, TimeSpanStyles.AssumeNegative, out end);
                    time[0] = Convert.ToInt32(start.TotalSeconds);
                    time[1] = Convert.ToInt32(end.TotalSeconds);

                    truth.Add(new KeyValuePair<int[], int>(time, Convert.ToInt32(items[2])));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //foreach (KeyValuePair<double, int> row in truth)
            //{
                

            //    Console.WriteLine(row.Key + ": " + row.Value);

            //}
            //Console.ReadLine();

            return truth;
        }

        public static List<List<string>> getInputData(int numSecs)
        {

            double currentTime = 0;
            List<List<string>> titles = new List<List<string>>();
            string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\window_titles\P04\appdata_fixed.csv";
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 1; i < 100; i++)
                {
                    string[] items = lines[i].Split(',');
                    double duration = Convert.ToDouble(items[1]) - Convert.ToDouble(items[0]);

                    int startBlock = (int)(currentTime / numSecs); // block of time in which the window title began
                    currentTime += duration;
                    int endBlock = (int)(currentTime / numSecs); // block of time in which the window title ended


                    //add the window title to all blocks of time it occured in 
                    for (int j = startBlock; j <= endBlock; j++)
                    {

                        if (j >= titles.Count) // if there is no block for this time window in the list
                        {
                            List<string> newBlock = new List<string>();
                            newBlock.Add(items[3]);
                            titles.Add(newBlock);
                        }
                        else
                        {

                            titles[j].Add(items[3]);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            int seconds = numSecs;

            foreach (List<string> list in titles)
            {
                TimeSpan time = TimeSpan.FromSeconds(seconds);
                string hours = time.ToString(@"hh\:mm\:ss");

                foreach (string token in list)
                {
                    Console.WriteLine(token);

                }
                seconds += numSecs;
                Console.WriteLine("---------------------------- " + hours);

            }

            return titles;

        }















    }
}
