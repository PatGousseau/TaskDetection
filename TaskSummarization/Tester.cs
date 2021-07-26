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
        private string participant;
        private Summarizer summarizer;
        private List<Task> tasks;
        private List<Task> groundTruthTasks;
        private Dictionary<int, Dictionary<int,int>> GTtaskMatchingDict;
        private Dictionary<int, Dictionary<int, int>> outputTaskMatchingDict;
        

        public Tester(int numSecs, double topPercentile, double similarityThreshold, string participant)
        {
            this.numSecs = numSecs;
            this.topPercentile = topPercentile;
            this.similarityThreshold = similarityThreshold;
            this.participant = participant;
            GTtaskMatchingDict = new Dictionary<int, Dictionary<int, int>>();
            outputTaskMatchingDict = new Dictionary<int, Dictionary<int, int>>();
            this.groundTruthTasks = new List<Task>();

            initializeSummarizer();

            this.tasks = summarizer.getTasks();
            getGroundTruthData();
        }

        /// <summary>
        /// Runs the algorithm
        /// </summary>
        /// <returns></returns>
        private void initializeSummarizer()
        {

            List<List<string>> data =     getInputData(numSecs); //getPersonalInputData(numSecs);
            this.summarizer = new Summarizer(numSecs,similarityThreshold);

            foreach (List<string> titles in data)
            {
                Task task = new Task(titles, topPercentile);
               
                summarizer.addTask(task);

            }
            summarizer.printData();

        }

        /// <summary>
        /// Runs the horizontal, vertical and task association tests
        /// </summary>
        /// <returns>float[] - First item: horizontal metric, Second item: vertical metric</returns>
        public float[] test()
        {
            float[] results = new float[2];
            float correctHorizPoints = 0;
            float correctVertPoints = 0;
          
            foreach (Task GTtask in groundTruthTasks)
            {
                int startSecs = GTtask.getStartTime();
                TimeSpan startTime = TimeSpan.FromSeconds(startSecs);
                string startHours = startTime.ToString(@"hh\:mm\:ss");

                int endSecs = GTtask.getEndTime();
                TimeSpan endTime = TimeSpan.FromSeconds(endSecs);
                string endHours = endTime.ToString(@"hh\:mm\:ss");


                // Populate GTtaskMatchingDict

                int outputTaskNum = mostCommonTaskNumInInterval(startSecs, endSecs, tasks); // Most common task number in the output within the interval of GTtask

                if(GTtaskMatchingDict.ContainsKey(GTtask.getTaskNum()))
                {
                    addToDict(GTtaskMatchingDict[GTtask.getTaskNum()],outputTaskNum);
                } else
                {
                    GTtaskMatchingDict.Add(GTtask.getTaskNum(), new Dictionary<int, int> { { outputTaskNum, 1 } });
                }

                // Vertical and Horizontal test
    
                //correctVertPoints += verticalTest(startSecs, endSecs, GTtask.Value);
                //correctHorizPoints += horizTest(startSecs, endSecs);
            }
            matchGtToOutput();


            //float horizontalAccuracy = correctHorizPoints / correctData.Count;
            //float verticalAccuracy = correctVertPoints / correctData.Count;
            //results[0] = horizontalAccuracy;
            //results[1] = verticalAccuracy;

            results[0] = taskAssociationTest(GTtaskMatchingDict);
            results[1] = taskAssociationTest(outputTaskMatchingDict);
            return results;
        }

        private void print(Dictionary<int, Dictionary<int, int>> dict)
        {
            foreach(KeyValuePair<int,Dictionary<int,int>> entry in dict)
            {
                
                foreach(KeyValuePair<int,int> corresp in entry.Value)
                {
                    Console.WriteLine(entry.Key + ": " + corresp.Key + " - " + corresp.Value + " times");
                }
            }
        }


        private void matchGtToOutput()
        {
            foreach(Task task in tasks)
            {
                int outputTaskNum = mostCommonTaskNumInInterval(task.getStartTime(), task.getEndTime(), groundTruthTasks); // Most common task number in GT within the interval of task

                if (outputTaskMatchingDict.ContainsKey(task.getTaskNum()))
                {
                    addToDict(outputTaskMatchingDict[task.getTaskNum()], outputTaskNum);
                }
                else
                {
                    outputTaskMatchingDict.Add(task.getTaskNum(), new Dictionary<int, int> { { outputTaskNum, 1 } });
                }
            }
        }

        private void addToDict(Dictionary<int,int> dict,int toAdd)
        {
            if (dict.ContainsKey(toAdd))
            {
                dict[toAdd] = dict[toAdd] + 1;
               
            }
            else
            {
                dict.Add(toAdd, 1);
            }
        }

        private float taskAssociationTest(Dictionary<int, Dictionary<int, int>>  dict)
        {
            float correctPoints = 0;
            float total = 0;
            foreach (KeyValuePair<int,Dictionary<int,int>> task in dict)
            {
                int max = 0;
                
                foreach(KeyValuePair<int,int> correspondingTaskNums in task.Value)
                {
                    total += correspondingTaskNums.Value;
                    if(correspondingTaskNums.Value >= max)
                    {
                        max = correspondingTaskNums.Value;
                    }
                }
                correctPoints += max;
            }

            float ans = correctPoints / total;
            return ans;

        }

        /// <summary>
        /// Returns whether or not the segment from startTime to endTime in 
        /// the correct data is homogenous (comprised of a single task)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private Task getTaskAtTime(int startTime, int endTime)
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

        //private int mostCommonTaskNumInGT(int outputStartTime, int outputEndTime)
        //{
        //    int mostCommon = -1;
        //    int currentMax = 0;
        //    foreach (KeyValuePair<int[],int> GTtask in correctData)
        //    {
        //        int GTStartTime = GTtask.Key[0];
        //        int GTputEndTime = GTtask.Key[1];
        //        int numTimeBlocks = 0;

        //        if (GTStartTime >= outputStartTime && GTputEndTime <= outputEndTime) // GT block is within output
        //        {
        //            numTimeBlocks = (GTputEndTime - GTStartTime) / numSecs;

        //        }
        //        else if (GTStartTime >= outputStartTime && GTStartTime <= outputEndTime) // GT block begins in output and ends after it
        //        {
        //            numTimeBlocks = (outputEndTime - GTStartTime) / numSecs;

        //        }
        //        else if (GTputEndTime >= outputStartTime && GTputEndTime <= outputEndTime) // GT block begins before output and ends inside of it
        //        {
        //            numTimeBlocks = (GTputEndTime - outputStartTime) / numSecs;

        //        }
        //        else if (GTStartTime <= outputStartTime && GTputEndTime >= outputEndTime) // GT block begins before output and ends after it
        //        {
        //            numTimeBlocks = (outputEndTime - outputStartTime) / numSecs;
        //        }

        //        if (numTimeBlocks > currentMax)
        //        {
        //            currentMax = numTimeBlocks;
        //            mostCommon = GTtask.Value;
        //        }
        //    }
        //    return mostCommon;
        //}

        private int mostCommonTaskNumInInterval(int GTStartTime, int GTEndTime, List<Task> taskList)
        {
            int mostCommon = -1;
            int currentMax = 0;
            foreach (Task task in taskList)
            {
                int outputStartTime = task.getStartTime();
                int outputEndTime = task.getEndTime();
                int numTimeBlocks = 0;

                if(outputStartTime >= GTStartTime && outputEndTime <= GTEndTime) // ouput block is within GT
                {
                    numTimeBlocks = (outputEndTime - outputStartTime) / numSecs;

                } else if (outputStartTime >= GTStartTime && outputStartTime <= GTEndTime) // output block begins in GT and ends after it
                {
                    numTimeBlocks = (GTEndTime - outputStartTime) / numSecs;

                } else if (outputEndTime >= GTStartTime && outputEndTime <= GTEndTime) // output block begins before GT and ends inside of it
                {
                    numTimeBlocks = (outputEndTime - GTStartTime) / numSecs;

                } else if(outputStartTime <= GTStartTime && outputEndTime >= GTEndTime) // output block begins before GT and ends after it
                {
                    numTimeBlocks = (GTEndTime - GTStartTime) / numSecs;
                }
                   
                if (numTimeBlocks > currentMax)
                {                        
                    currentMax = numTimeBlocks;
                    mostCommon = task.getTaskNum();
                }                   
            }
            return mostCommon;
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

        private int verticalTest(int startSecs, int endSecs, Task task)
        {
            int points = 0;
            if (isHomogeneous(startSecs + 150, endSecs - 150))
            {
                if (summarizer.similarTasks(getTaskAtTime(startSecs + 150, endSecs - 150), task, 0.3))
                {
                    points = 1;
                }
            }
            return points;
        }

        private int horizTest(int startSecs, int endSecs)
        {
            int points = 0;
            if (!isHomogeneous(endSecs - 150, endSecs + 150) && !isHomogeneous(startSecs - 150, startSecs + 150)) // within 5 minutes
            {
                if (isHomogeneous(startSecs + 150, endSecs - 150))
                {
                    points = 1;
                }
            }
            return points;
        }



        #region input/output


        /// <summary>
        /// Fetches ground truth data
        /// </summary>
        /// <returns></returns>
        public void getGroundTruthData()
        {
            string format = "g";
            var culture = CultureInfo.InvariantCulture;
            List<KeyValuePair<int[], int>> truth = new List<KeyValuePair<int[], int>>();
            string path = @"c:\users\pcgou\onedrive\documents\ubcresearch\window_titles\";
            path += participant;
            path += @"\truth.txt";
            //string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\window_titles\PD\truth.txt";
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 1; i < lines.Length; i++)
                {
                    
                    string[] items = lines[i].Split(',');
                    Task newTask = new Task(getGTDescriptions(Convert.ToInt32(items[2])),1);
                    TimeSpan start, end;
                    TimeSpan.TryParseExact(items[0], format, culture, TimeSpanStyles.AssumeNegative, out start);
                    TimeSpan.TryParseExact(items[1], format, culture, TimeSpanStyles.AssumeNegative, out end);
                    newTask.setTimes(Convert.ToInt32(start.TotalSeconds), Convert.ToInt32(end.TotalSeconds));
                    newTask.setTaskNum(Convert.ToInt32(items[2]));
                    groundTruthTasks.Add(newTask);
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
        }

        /// <summary>
        /// Fetches testing data
        /// </summary>
        /// <param name="numSecs"></param>
        /// <returns></returns>
        public List<List<string>> getInputData(int numSecs)
        {
            
            double currentTime = 0;
            List<List<string>> titles = new List<List<string>>();
            string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\window_titles\";
            path += participant;
            path += @"\appdata_fixed.csv";
            
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

        public List<List<string>> getPersonalInputData(int numSecs)
        {

            double currentTime = 0;
            List<List<string>> titles = new List<List<string>>();
            string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\window_titles\PD\personal_data2.txt";

            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] items = lines[i].Split(',');
                    double duration = Convert.ToDouble(items[2]);

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



            //foreach (List<string> list in titles)
            //{

            //    foreach (string token in list)
            //    {
            //        Console.WriteLine(token);


            //    }

            //    Console.WriteLine("-------------------------------");

            //}
            //Console.ReadLine();

            return titles;

        }



        /// <summary>
        /// Initalizes GroundTruthDescriptions
        /// </summary>
        /// <returns></returns>
        private List<string> getGTDescriptions(int taskNum)
        {
            switch (taskNum)
            {
                case 1:
                    return new List<string> { "Your manager has noticed that there has been a substantial influx of duplicate bug reports recently. Explore the provided list of bug reports, and identify whether there is a duplicate and provide its ID. search the bugs with the ids: 2264, 2268, 2271, 2277" };
                case 2:
                    return new List<string> { "Your team has developed an application for optimizing developer work patterns - reducing the number of impactful interruptions. You have been tasked with creating visualizations for a presentation outlining the benefits of your product to potential clients. You have the following data available to you for use: Application usage times, interruption times, durations, and disruptiveness levels, keyboard and mouse activity levels. What libraries would you suggest for creating data representations? Give some examples of other works that have been created using these libraries. At a minimum the visualizations should include a before/after comparison of developers work days using the product vs not using the product." };
                case 3:
                    return new List<string> { "The software company you work for is considering expanding into the productivity tool sphere. Your manager has asked you to do some market research on 3 of the most popular already existing apps in this domain: Microsoft To-do, Wunderlist, and Todoist. Provide a short written summary of the similarities and differences between these 3 apps." };
                case 4:
                    return new List<string> { "Your coworker is having difficulty deciding on which productivity app they should use, and have asked you for a recommendation. They have narrowed their decision to 3 apps: Microsoft To-do, Wunderlist, and Todoist. Based only on app store reviews, which of these apps would you recommend? Identify any reviews that were particularly influential in your decision." };
                case 5:
                    return new List<string> { "You are preparing to give a presentation on potential deep learning applications to the CTO of your company. While you have already completed the slides for the presentation, you should also prepare answers for a few questions which are likely to arise during the presentation. The lines drawn between layers of the network on the included slide represent weighted inputs from one layer to the next. How does the network decide on what weights to choose during the training process? Most of the technologies behind deep learning have already been around for over 30 years.Why is deep learning only becoming popular now ? What has changed ? What kind of performance increases can be seen by using GPU's instead of CPU's? Are GPU's always superior with respect to deep learning applications?" };
                case 6:
                    return new List<string> { "You recently gave a short presentation to your colleagues outlining different ways the company may be able to make use of blockchain. One of your co-workers felt a little bit lost during the presentation, and emailed you a couple follow up questions afterwards. You mentioned that blockchain is a form of distributed ledger. What does this mean? What advantages are offered over traditional client-server database ledger systems? Could you explain what proof-of-work means? What is it? What does it do? Why is it necessary ?" };
                default:
                    return null;
            }
        }

        #endregion
    }


}
