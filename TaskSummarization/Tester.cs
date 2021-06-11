using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


namespace TaskSummarization
{
    class Tester
    {

        private int numSecs;
        private double topPercentile;
        private double similarityThreshold;
        private Summarizer summarizer;
        private List<int[]> taskSwitchTimes;
        private List<KeyValuePair<BagOfWords, int>> bags;
        private List<KeyValuePair<int[], int>> correctData;
        public Tester(int numSecs, double topPercentile, double similarityThreshold)
        {
            this.numSecs = numSecs;
            this.topPercentile = topPercentile;
            this.similarityThreshold = similarityThreshold;

            init();
            this.taskSwitchTimes = summarizer.getTimes();
            this.bags = summarizer.getBags();
            this.correctData = getCorrectData();

        }

        private void init()
        {

            List<List<string>> data = getInputData(numSecs);
            this.summarizer = new Summarizer(numSecs,similarityThreshold);

            foreach (List<string> titles in data)
            {
                BagOfWords bag = new BagOfWords(titles, topPercentile);
                summarizer.addBag(bag);

            }
            summarizer.printData();

        }

        private int getBagNumber(int startTime, int endTime)
        {
            List<int[]> taskSwitchTimes = summarizer.getTimes();
            int bagNum = 0;
            foreach(int[] task in taskSwitchTimes)
            {
                if(task[0] <= startTime && task[1] >= endTime)
                {
                    return bagNum;
                }
                bagNum++;
            }
            return -1;
        }



        public float test()
        {

            float correctPoints = 0;
            
            foreach (int[] time in taskSwitchTimes)
            {
                int startSecs = time[0];
                int endSecs = time[1];
                TimeSpan startTime = TimeSpan.FromSeconds(startSecs);
                string startHours = startTime.ToString(@"hh\:mm\:ss");

                TimeSpan endTime = TimeSpan.FromSeconds(endSecs);
                string endHours = endTime.ToString(@"hh\:mm\:ss");

                if (isHomogeneous(startSecs, endSecs))
                {
                    correctPoints++;
                    Console.WriteLine("correct from: " + startHours + " to " + endHours); 
                } else
                {
                    Console.WriteLine("wrong from: " + startHours + " to " + endHours);
                }
            }
            float ans = correctPoints / taskSwitchTimes.Count;
            return ans;
        }



        //public float test()
        //{
        //    float correctPoints = 0;
        //    float totalPoints = 0;
        //    float ans = 0;


        //    int curTime = 0; // Current time

        //    // loop through correctData
        //    foreach(KeyValuePair<int[], int> correctTask in correctData)
        //    {
        //        int upperBound = correctTask.Key[1];
        //        var freq = new Dictionary<int, int>(); // frequency of each task number
        //        int index = 0;
        //        while ((curTime + numSecs) <= upperBound)
        //        {
        //            int bagNum = getBagNumber(curTime, curTime + numSecs);
        //            if (bagNum >= 0) {
        //                int endTime = curTime + numSecs;
        //                TimeSpan ctime = TimeSpan.FromSeconds(curTime);
        //                string chours = ctime.ToString(@"hh\:mm\:ss");
        //                TimeSpan etime = TimeSpan.FromSeconds(endTime);
        //                string ehours = etime.ToString(@"hh\:mm\:ss");
        //              //  Console.WriteLine(chours + " to " + ehours + " : " + bags[bagNum].Value);
        //                int taskNum = bags[bagNum].Value;
        //                if (freq.ContainsKey(taskNum))
        //                {
        //                    freq[taskNum] = freq[taskNum] + 1;
        //                } else
        //                {
        //                    freq.Add(taskNum, 1);
        //                }

        //                totalPoints++;
        //            }
        //            index++;
        //            curTime += numSecs;
        //        }
        //        curTime += numSecs;
        //        if (freq.Count > 0)
        //        {
        //            correctPoints += freq.Values.Max();
        //        }
        //     //   Console.WriteLine(correctPoints + " / " + totalPoints);
        //    }
        //     ans = correctPoints / totalPoints;

        //    return ans;
        //}




        private bool isHomogeneous(int startTime, int endTime)
        {

            for(int i = 0; i < correctData.Count; i++)
            {
                if(startTime >= correctData[i].Key[0] && endTime <= correctData[i].Key[1])
                {
                    //return correctData[i].Value;
                    return true;
                }
            }
            //return -1; // transition period
            return false;


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

            //foreach (List<string> list in titles)
            //{
            //    TimeSpan time = TimeSpan.FromSeconds(seconds);
            //    string hours = time.ToString(@"hh\:mm\:ss");

            //    foreach (string token in list)
            //    {
            //        Console.WriteLine(token);

            //    }
            //    seconds += numSecs;
            //    Console.WriteLine("---------------------------- " + hours);

            //}

            return titles;

        }
    }
}
