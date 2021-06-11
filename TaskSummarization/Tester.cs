using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace TaskSummarization
{
    class Tester
    {

        private int numSecs;
        private double topPercentile;
        private double similarityThreshold;
        private Summarizer summarizer;
        private List<Task> tasks;
        private List<KeyValuePair<int[], int>> correctData;

        public Tester(int numSecs, double topPercentile, double similarityThreshold)
        {
            this.numSecs = numSecs;
            this.topPercentile = topPercentile;
            this.similarityThreshold = similarityThreshold;

            initializeSummarizer();
            
            this.tasks = summarizer.getTasks();
            this.correctData = getCorrectData();

        }

        /// <summary>
        /// Runs the algorithm
        /// </summary>
        /// <returns></returns>
        private void initializeSummarizer()
        {

            List<List<string>> data = getInputData(numSecs);
            this.summarizer = new Summarizer(numSecs,similarityThreshold);

            foreach (List<string> titles in data)
            {
                Task task = new Task(titles, topPercentile);
                summarizer.addTask(task);

            }
            summarizer.printData();

        }

        /// <summary>
        /// Tests the algorithm
        /// </summary>
        /// <returns></returns>
        public float test()
        {

            float correctPoints = 0;
            
            foreach (Task task in tasks)
            {
                int startSecs = task.getStartTime();
                TimeSpan startTime = TimeSpan.FromSeconds(startSecs);
                string startHours = startTime.ToString(@"hh\:mm\:ss");

                int endSecs = task.getEndTime();
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
            float ans = correctPoints / tasks.Count;
            return ans;
        }


        /// <summary>
        /// Returns whether or not the segment from startTime to endTime in 
        /// the correct data is homogenous (comprised of a single task)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Fetches ground truth data
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Fetches testing data
        /// </summary>
        /// <param name="numSecs"></param>
        /// <returns></returns>
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
