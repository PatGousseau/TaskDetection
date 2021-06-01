﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSummarization
{
    class Program
    {

         


        static void Main(string[] args)
        {

            int numSecs = 90;
            double topPercentile = 0.4;

            List<List<string>> data = getTestData(numSecs);
            Summarizer summarizer = new Summarizer();

            foreach(List<string> titles in data)
            {
                BagOfWords bag = new BagOfWords(titles, topPercentile);
                summarizer.addBag(bag);

            }
            summarizer.printData();

            
 

            //List<Dictionary<string, int>> bags = summarizer.getBags().getBags();
            //foreach (Dictionary<string, int> dict in bags)
            //{

            //    foreach (KeyValuePair<string,int> token in dict)
            //    {
            //        Console.WriteLine(token.Key +": " + token.Value);

            //    }
            //    Console.WriteLine("----------------------------");
            //}
            //Console.ReadLine();




        }

        public static List<List<string>> getTestData(int numSecs)
        {

            double currentTime = 0;
            List<List<string>> titles = new List<List<string>>();
            string path = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\window_titles\P01\appdata_fixed.csv";
            try
            {
                string[] lines =  File.ReadAllLines(path);
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] items = lines[i].Split(',');
                    //double x = Convert.ToDouble(items[1]);
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



                } catch (Exception e)
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
           // Console.ReadLine();

            return titles;

        }
    }
}
