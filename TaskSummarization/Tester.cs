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
        private Dictionary<int, Task> GroundTruthDescriptions;
        

        public Tester(int numSecs, double topPercentile, double similarityThreshold)
        {
            this.numSecs = numSecs;
            this.topPercentile = topPercentile;
            this.similarityThreshold = similarityThreshold;
            GroundTruthDescriptions = new Dictionary<int, Task>();

            initializeSummarizer();
            setGTDescriptions();


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
        /// <returns>float[] - First item: horizontal metric, Second item: vertical metric</returns>
        public float test()
        {
            float[] results = new float[2];
            float correctHorizPoints = 0;
            double numCorrectTrans = 0;

            Dictionary<int, List<KeyValuePair<int[], int>>> correctDict = new Dictionary<int, List<KeyValuePair<int[], int>>>();


            foreach (KeyValuePair<int[], int>  GTtask in correctData)
            {
                int startSecs = GTtask.Key[0];
                TimeSpan startTime = TimeSpan.FromSeconds(startSecs);
                string startHours = startTime.ToString(@"hh\:mm\:ss");

                int endSecs = GTtask.Key[1];
                TimeSpan endTime = TimeSpan.FromSeconds(endSecs);
                string endHours = endTime.ToString(@"hh\:mm\:ss");

                // Add all correct task segments to a dictionnary
                if(correctDict.ContainsKey(GTtask.Value))
                {
                    correctDict[GTtask.Value].Add(GTtask);
                } else
                {
                    correctDict.Add( GTtask.Value, new List<KeyValuePair<int[], int>> { GTtask });
                }
                


                // check if transition period also occurs in output
                if(!isHomogeneous(GTtask.Key[1] - 60, GTtask.Key[1] + 60)) // within 2 minutes
                {
                    Console.WriteLine("transition: " + endHours + " - 60 to  " + endHours + " + 60");
                    numCorrectTrans++;
                }
            }




            double transitionAccuracy = numCorrectTrans / correctData.Count - 1;
            float horizontalAccracy = correctHorizPoints / tasks.Count;
            results[0] = horizontalAccracy;
            return horizontalAccracy;
        }



        private void taskMatching(Dictionary<int, List<KeyValuePair<int[], int>>> correctDict, Dictionary<int, List<Task>> outputDict)
        {
            // take one item from correect dict
            //
            
        }


    private bool isHomogeneous(int startTime, int endTime)
        {

            foreach (Task task in tasks)
            {
                if (startTime >= task.getStartTime() && endTime <= task.getEndTime())
                {
                    return true;
                }
            }
            return false; // transition period

        }


        /// <summary>
        /// Returns whether or not the segment from startTime to endTime in 
        /// the correct data is homogenous (comprised of a single task)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        //private bool isHomogeneous(int startTime, int endTime)
        //{

        //    for (int i = 0; i < correctData.Count; i++)
        //    {
        //        if (startTime >= correctData[i].Key[0] && endTime <= correctData[i].Key[1])
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// Tests the algorithm
        ///// </summary>
        ///// <returns>float[] - First item: horizontal metric, Second item: vertical metric</returns>
        //public float test()
        //{
        //    float[] results = new float[2]; 
        //    float correctHorizPoints = 0;

        //    foreach (Task task in tasks)
        //    {
        //        int startSecs = task.getStartTime();
        //        TimeSpan startTime = TimeSpan.FromSeconds(startSecs);
        //        string startHours = startTime.ToString(@"hh\:mm\:ss");

        //        int endSecs = task.getEndTime();
        //        TimeSpan endTime = TimeSpan.FromSeconds(endSecs);
        //        string endHours = endTime.ToString(@"hh\:mm\:ss");

        //        if (isHomogeneous(startSecs, endSecs))
        //        {
        //            if(summarizer.similarTasks(task,GroundTruthDescriptions[getCorrectTaskNum(startSecs, endSecs)], 0.3))
        //            {
        //                correctHorizPoints++;
        //                //Console.WriteLine("correct from: " + startHours + " to " + endHours);
        //            } else
        //            {
        //                //Console.WriteLine("wrong from: " + startHours + " to " + endHours + " cosine" );
        //            }


        //        } else
        //        {
        //            //Console.WriteLine("wrong from: " + startHours + " to " + endHours + " homo");
        //        }
        //    }
        //    float horizontalAccracy = correctHorizPoints / tasks.Count;
        //    results[0] = horizontalAccracy;
        //    return horizontalAccracy;
        //}


        ///// <summary>
        ///// Returns whether or not the segment from startTime to endTime in 
        ///// the correct data is homogenous (comprised of a single task)
        ///// </summary>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //private bool isHomogeneous(int startTime, int endTime)
        //{

        //    for(int i = 0; i < correctData.Count; i++)
        //    {
        //        if(startTime >= correctData[i].Key[0] && endTime <= correctData[i].Key[1])
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// Returns whether or not the segment from startTime to endTime in 
        /// the correct data is homogenous (comprised of a single task)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        //private int getCorrectTaskNum(int startTime, int endTime)
        //{

        //    for (int i = 0; i < correctData.Count; i++)
        //    {
        //        if (startTime >= correctData[i].Key[0] && endTime <= correctData[i].Key[1])
        //        {
        //            return correctData[i].Value;
        //        }
        //    }
        //    return -1; // transition period
        //}

        private Task getTaskNum(int startTime, int endTime)
        {

            foreach (Task task in tasks)
            {
                if (startTime >= task.getStartTime() && endTime <= task.getEndTime())
                {
                    return task;
                }
            }
            return null; // transition period
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
                for (int i = 1; i < lines.Length; i++)
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

        /// <summary>
        /// Initalizes GroundTruthDescriptions
        /// </summary>
        /// <returns></returns>
        private void setGTDescriptions()
        {
            List<string> descriptions = new List<string>();
            Task task1 = new Task(new List<string> { "Your manager has noticed that there has been a substantial influx of duplicate bug reports recently. Explore the provided list of bug reports, and identify whether there is a duplicate and provide its ID. search the bugs with the ids: 2264, 2268, 2271, 2277" }, 1);
            Task task2 = new Task(new List<string> { "Your team has developed an application for optimizing developer work patterns - reducing the number of impactful interruptions. You have been tasked with creating visualizations for a presentation outlining the benefits of your product to potential clients. You have the following data available to you for use: Application usage times, interruption times, durations, and disruptiveness levels, keyboard and mouse activity levels. What libraries would you suggest for creating data representations? Give some examples of other works that have been created using these libraries. At a minimum the visualizations should include a before/after comparison of developers work days using the product vs not using the product." }, 1);
            Task task3 = new Task(new List<string> { "The software company you work for is considering expanding into the productivity tool sphere. Your manager has asked you to do some market research on 3 of the most popular already existing apps in this domain: Microsoft To-do, Wunderlist, and Todoist. Provide a short written summary of the similarities and differences between these 3 apps." }, 1);
            Task task4 = new Task(new List<string> { "Your coworker is having difficulty deciding on which productivity app they should use, and have asked you for a recommendation. They have narrowed their decision to 3 apps: Microsoft To-do, Wunderlist, and Todoist. Based only on app store reviews, which of these apps would you recommend? Identify any reviews that were particularly influential in your decision." }, 1);
            Task task5 = new Task(new List<string> { "You are preparing to give a presentation on potential deep learning applications to the CTO of your company. While you have already completed the slides for the presentation, you should also prepare answers for a few questions which are likely to arise during the presentation. The lines drawn between layers of the network on the included slide represent weighted inputs from one layer to the next. How does the network decide on what weights to choose during the training process? Most of the technologies behind deep learning have already been around for over 30 years.Why is deep learning only becoming popular now ? What has changed ? What kind of performance increases can be seen by using GPU's instead of CPU's? Are GPU's always superior with respect to deep learning applications?" }, 1);
            Task task6 = new Task(new List<string> { "You recently gave a short presentation to your colleagues outlining different ways the company may be able to make use of blockchain. One of your co-workers felt a little bit lost during the presentation, and emailed you a couple follow up questions afterwards. You mentioned that blockchain is a form of distributed ledger. What does this mean? What advantages are offered over traditional client-server database ledger systems? Could you explain what proof-of-work means? What is it? What does it do? Why is it necessary ?" }, 1);

            this.GroundTruthDescriptions.Add(1, task1);
            this.GroundTruthDescriptions.Add(2, task2);
            this.GroundTruthDescriptions.Add(3, task3);
            this.GroundTruthDescriptions.Add(4, task4);
            this.GroundTruthDescriptions.Add(5, task5);
            this.GroundTruthDescriptions.Add(6, task6);
        }
    }
}
