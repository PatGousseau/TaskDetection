using System;
using System.Collections.Generic;
using System.IO;


namespace TaskSummarization
{
    class Program
    {

        
        static void Main(string[] args)
        {
            quickTest();

        }


        private static void quickTest()
        {
            int numSecs = 90;
            double similarity = 0.45;
            double topPercentile = 0.4;



            Tester tester = new Tester(numSecs, topPercentile, similarity);
            float accuracy = tester.test();
            Console.WriteLine(accuracy);

            Console.ReadLine();
        }

        private static void megaTest()
        {
            int startNumSecs = 30;
            double startSimilarity = 0.3;
            double topPercentile = 0.4;
            string id = "P04";

            string filePath = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\megaTestP04.txt";
            List<string> lines = new List<string>();
            lines.Add("Participant ID: " + id);

            for (int numSecs = startNumSecs; numSecs <= 180; numSecs += 30) // 30 to 180
            {
                for (double similarity = startSimilarity; similarity <= 0.4; similarity += 0.1) // 0.3 to 0.6
                {

                    Tester tester = new Tester(numSecs, topPercentile, similarity);
                    float accuracy = tester.test();
                    string toAdd = "number of seconds: " + numSecs + ", similarity threshold: " + similarity + ", accuracy: " + accuracy;
                    lines.Add(toAdd);
                    Console.WriteLine("X");
                }
            }
            File.WriteAllLines(filePath, lines);
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
