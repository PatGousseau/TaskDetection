using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TaskSummarization
{
    class Program
    {

        private static List<string> participantIDs = new List<string>();
        
        static void Main(string[] args)
        {

            megaTest();
            //quickTest();
            //testPD();

            //string ans = Regex.Replace(Regex.Replace("task", @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            //Console.WriteLine(ans);
            //Console.ReadLine();




            //string filePath = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\Tests\perfect\average.txt";

            //string x = System.IO.Path.GetDirectoryName(filePath);
            //DirectoryInfo y = System.IO.Directory.GetParent(filePath);
            //Console.WriteLine(y.FullName);

            //Console.ReadLine();
        }


        private static void testPD()
        {

            string filePath = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\Tests\perfect\average.txt";
            Console.WriteLine("Enter duration of each timeblock:");
            string strNumSecs = Console.ReadLine();
            string[] numSecs = strNumSecs.Split(',');

            Console.WriteLine("Enter similary thresholds seperated by commas:");
            string strSimilarities = Console.ReadLine();
            string[] similarities = strSimilarities.Split(',');


            double topPercentile = 0.4;
            double soFar = 0;
            double total = similarities.Length * numSecs.Length;



            for (int j = 0; j < numSecs.Length; j++)
            {
                int curNumSecs = Convert.ToInt32(numSecs[j]);
                for (int i = 0; i < similarities.Length; i++) 
                {

                    Tester tester = new Tester(curNumSecs, topPercentile, Convert.ToDouble(similarities[i]), "P04");
                    float[] accuracy = tester.test();
                    string toAdd =  "Personal data: number of seconds: " + curNumSecs + ", top % of words: " + topPercentile + ", similarity threshold: " + similarities[i] + " , horiz accuracy: " + accuracy[0] + ", vert accuracy: " + accuracy[1];
                    soFar++;
                    double progress = (soFar / total);
                    int perc = (int)(progress * 100);
                    Console.WriteLine(perc + "% complete");
                   File.AppendAllText(filePath, toAdd + Environment.NewLine);
                }
            }
            

            Console.WriteLine("Done!");
            Console.ReadLine();
        }


        private static void quickTest()
        {
            int numSecs = 120;
            double similarity = 0.35;
            double topPercentile = 0.4;



            Tester tester = new Tester(numSecs, topPercentile, similarity,"P04");
            float[] accuracy = tester.test();
          Console.WriteLine("h: " + accuracy[0]);
            Console.WriteLine("v: " + accuracy[1]);

            Console.ReadLine();
        }

        private static void addIDs()
        {
            //participantIDs.Add("P01");
            //participantIDs.Add("P02");
            //participantIDs.Add("P03");
            //participantIDs.Add("P04");
            //participantIDs.Add("P05");
            //participantIDs.Add("P06");
            //participantIDs.Add("P14");
            //participantIDs.Add("P15");
            //participantIDs.Add("P16");
            participantIDs.Add("P18");
        }

        private static void megaTest()
        {
            addIDs();

            string filePath = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\Tests\perfect\average.txt";

            Console.WriteLine("Enter duration of each timeblock:");
            string strNumSecs = Console.ReadLine();
            string[] numSecs = strNumSecs.Split(',');

            Console.WriteLine("Enter similary thresholds seperated by commas:");
            string strSimilarities = Console.ReadLine();
            string[] similarities = strSimilarities.Split(',');

            //File.AppendAllText(filePath, "Participant ID: P14" + Environment.NewLine);


            double topPercentile = 0.4;
            
            double soFar = 0;
            double total = similarities.Length * numSecs.Length * participantIDs.Count;

            for (int k = 0; k < participantIDs.Count; k++) {

                for (int j = 0; j < numSecs.Length; j++) // 30 to 180
                {
                    int curNumSecs = Convert.ToInt32(numSecs[j]);
                    for (int i = 0; i < similarities.Length; i++) // 0.2 to 0.5
                    {

                        Tester tester = new Tester(curNumSecs, topPercentile, Convert.ToDouble(similarities[i]), participantIDs[k]);
                        float[] accuracy = tester.test();
                        string toAdd = participantIDs[k] + ": number of seconds: " + curNumSecs + ", top % of words: " + topPercentile + ", similarity threshold: " + similarities[i] + " , horiz accuracy: " + accuracy[0] + ", vert accuracy: " + accuracy[1];
                        soFar++;
                        double progress = (soFar / total);
                        int perc = (int)(progress * 100);
                        Console.WriteLine(perc + "% complete");
                        File.AppendAllText(filePath, toAdd + Environment.NewLine);
                    }
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
