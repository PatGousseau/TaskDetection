using System;
using System.Collections.Generic;
using System.IO;


namespace TaskSummarization
{
    class Program
    {

        
        static void Main(string[] args)
        {

            //megaTest();
            quickTest();
        }


        private static void quickTest()
        {
            int numSecs = 60;
            double similarity = 0.2;
            double topPercentile = 0.4;



            Tester tester = new Tester(numSecs, topPercentile, similarity);
            float accuracy = tester.test();
            Console.WriteLine(accuracy);

            Console.ReadLine();
        }

        private static void megaTest()
        {

            string filePath = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\megaTestP04.txt";

            Console.WriteLine("Enter duration of each timeblock:");
            string strNumSecs = Console.ReadLine();
            string[] numSecs = strNumSecs.Split(',');
            Console.WriteLine("Enter similary thresholds seperated by commas:");
            string strSimilarities = Console.ReadLine();
            string[] similarities = strSimilarities.Split(',');

            File.AppendAllText(filePath, "Participant ID: P04" + Environment.NewLine);


            double topPercentile = 0.4;
            string id = "P04";


            List<string> lines = new List<string>();
            lines.Add("Participant ID: " + id);
            double soFar = 0;
            double total = similarities.Length;

            for (int j = 0; j < numSecs.Length; j++) // 30 to 180
            {
                int curNumSecs = Convert.ToInt32(numSecs[j]);
                for (int i = 0; i < similarities.Length; i++) // 0.2 to 0.5
                {

                    Tester tester = new Tester(curNumSecs, topPercentile, Convert.ToDouble(similarities[i]));
                    float accuracy = tester.test();
                    string toAdd = "number of seconds: " + curNumSecs + ", top % of words: " + topPercentile + ", similarity threshold: " + similarities[i] + ", accuracy: " + accuracy;
                    lines.Add(toAdd);
                    soFar++;
                   double progress = (soFar / total);
                    int perc =(int) (progress * 100);
                    Console.WriteLine(perc + "% complete");
                    File.AppendAllText(filePath, toAdd + Environment.NewLine);
                }
            }
            
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        //public static List<List<string>> getTestData(int numSecs)
        //{

        //    double currentTime = 0;
        //    List<List<string>> titles = new List<List<string>>();
        //    string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\window_titles\P04\appdata_fixed.csv";
        //    try
        //    {
        //        string[] lines =  File.ReadAllLines(path);
        //        for (int i = 1; i < 7; i++)
        //        {
        //            string[] items = lines[i].Split(',');
        //            double duration = Convert.ToDouble(items[1]) - Convert.ToDouble(items[0]);

        //            int startBlock = (int)(currentTime / numSecs); // block of time in which the window title began
        //            currentTime += duration;
        //            int endBlock = (int)(currentTime / numSecs); // block of time in which the window title ended


        //            //add the window title to all blocks of time it occured in 
        //            for (int j = startBlock; j <= endBlock; j++)
        //            {

        //                if (j >= titles.Count) // if there is no block for this time window in the list
        //                {
        //                    List<string> newBlock = new List<string>();
        //                    newBlock.Add(items[3]);
        //                    titles.Add(newBlock);
        //                }
        //                else
        //                {

        //                    titles[j].Add(items[3]);
        //                }
        //            }
        //        }

        //    } catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }

        //    int seconds = numSecs;
            
        //    foreach (List<string> list in titles)
        //    {
        //        TimeSpan time = TimeSpan.FromSeconds(seconds);
        //        string hours = time.ToString(@"hh\:mm\:ss");

        //        foreach (string token in list)
        //        {
        //            Console.WriteLine(token);

        //        }
        //        seconds += numSecs;
        //        Console.WriteLine("---------------------------- " + hours);
                
        //    }

        //    return titles;

        //}
    }
}
