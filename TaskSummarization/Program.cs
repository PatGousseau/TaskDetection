using System;
using System.Collections.Generic;
using System.IO;

namespace TaskSummarization
{
    class Program
    {

        private static List<string> participantIDs = new List<string>();

        static void Main(string[] args)
        { }
        

        private static void addIDs()
        {
            participantIDs.Add("P01");
            participantIDs.Add("P02");
            participantIDs.Add("P03");
            participantIDs.Add("P04");
            participantIDs.Add("P05");
            participantIDs.Add("P06");
            participantIDs.Add("P14");
            participantIDs.Add("P15");
            participantIDs.Add("P16");
            participantIDs.Add("P18");
        }

        private static void megaTest()
        {
            addIDs();
            string filePath = @"C:\Users\pcgou\OneDrive\Documents\UBCResearch\Tests\perfect\task_association.txt";
            Console.WriteLine("Enter duration of each timeblock:");
            string strNumSecs = Console.ReadLine();
            string[] numSecs = strNumSecs.Split(',');
            Console.WriteLine("Enter similary thresholds seperated by commas:");
            string strSimilarities = Console.ReadLine();
            string[] similarities = strSimilarities.Split(',');
            double topPercentile = 0.4;
            double soFar = 0;
            double total = similarities.Length * numSecs.Length * participantIDs.Count;

            for (int k = 0; k < participantIDs.Count; k++)
            {
                for (int j = 0; j < numSecs.Length; j++) // 30 to 180
                {
                    int curNumSecs = Convert.ToInt32(numSecs[j]);
                    for (int i = 0; i < similarities.Length; i++) // 0.2 to 0.5
                    {
                        Tester tester = new Tester(curNumSecs, topPercentile, Convert.ToDouble(similarities[i]), participantIDs[k]);
                        float[] accuracy = tester.test();
                        string toAdd = participantIDs[k] + ": number of seconds: " + curNumSecs + ", top % of words: " + topPercentile + ", similarity threshold: " + similarities[i] + " , GT accuracy: " + accuracy[0] + " , output accuracy: " + accuracy[1];
                        soFar++;
                        double progress = (soFar / total);
                        int perc = (int)(progress * 100);
                        Console.WriteLine(perc + "% complete");
                        File.AppendAllText(filePath, toAdd + Environment.NewLine);
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
