using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSummarization
{
    class Program
    {
        static void Main(string[] args)
        {
 
            List<string> a = new List<string>() { "AM.PA.MonitoringTool (Running) - Microsoft Visual Studio", "New notification", "AM.PA.MonitoringTool (Running) - Microsoft Visual Studio", "Untitled - Google Chrome", "Your Retrospection for the 5/21/2021 - Google Chrome------AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---------PersonalAnalytics: Retrospection & Insights---Your Retrospection for the 5/21/2021 - Google Chrome---file io visual studio does not work if non console application - Google Search - Google Chrome---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---New notification---------PersonalAnalytics: Retrospection & Insights" };
            List<string> b = new List<string>() { "PersonalAnalytics: Retrospection & Insights---file io visual studio does not work if non console application - Google Search - Google Chrome---C# Write To File - Google Chrome---file io visual studio does not work if non console application - Google Search - Google Chrome---Enumerable.ToList(IEnumerable) Method (System.Linq) | Microsoft Docs - Google Chrome" };
            List<string> c = new List<string>() { "Enumerable.ToList(IEnumerable) Method (System.Linq) | Microsoft Docs - Google Chrome---PersonalAnalytics: Retrospection & Insights" };
            List<string> d = new List<string>() { "PersonalAnalytics: Retrospection & Insights---Enumerable.ToList(IEnumerable) Method (System.Linq) | Microsoft Docs - Google Chrome" };
            List<string> e = new List<string>() { "Enumerable.ToList(IEnumerable) Method (System.Linq) | Microsoft Docs - Google Chrome---csatterfield/TaskDetection-analysis - Google Chrome" };
            List<string> f = new List<string>() { "csatterfield/TaskDetection-analysis - Google Chrome---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---New notification---------PersonalAnalytics: Retrospection & Insights---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---csatterfield/TaskDetection-analysis - Google Chrome--- | Proceedings of the SIGCHI Conference on Human Factors in Computing Systems - Google Chrome---Cosine similarity - Wikipedia - Google Chrome" };
            List<string> g = new List<string>() { "Cosine similarity - Wikipedia - Google Chrome" };
            List<string> h = new List<string>() { "Cosine similarity - Wikipedia - Google Chrome--- | Proceedings of the SIGCHI Conference on Human Factors in Computing Systems - Google Chrome---visual studio print to console on non console application c# - Google Search - Google Chrome---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---New notification" };
            List<string> i = new List<string>() { "New notification---------PersonalAnalytics: Retrospection & Insights---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---New notification---------PersonalAnalytics: Retrospection & Insights---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio" };
            List<string> j = new List<string>() { "AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---visual studio print to console on non console application c# - Google Search - Google Chrome---C# Read and Write to a Text File - YouTube - Google Chrome---visual studio print to console on non console application c# - Google Search - Google Chrome--- | Proceedings of the SIGCHI Conference on Human Factors in Computing Systems - Google Chrome" };
            List<string> k = new List<string>() { " | Proceedings of the SIGCHI Conference on Human Factors in Computing Systems - Google Chrome---visual studio print to console on non console application c# - Google Search - Google Chrome---Enumerable.ToList<TSource>(IEnumerable<TSource>) Method (System.Linq) | Microsoft Docs - Google Chrome---c# - What is the best way to iterate over a dictionary? - Stack Overflow - Google Chrome---c# - How do you sort a dictionary by value? - Stack Overflow - Google Chrome---New Tab - Google Chrome---Outlook - Google Chrome---Loading... - Google Chrome---Mail - pgousse@student.ubc.ca - Google Chrome---c# - How do you sort a dictionary by value? - Stack Overflow - Google Chrome" };
            List<string> l = new List<string>() { "c# - How do you sort a dictionary by value? - Stack Overflow - Google Chrome---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---New notification---------PersonalAnalytics: Retrospection & Insights---AM.PA.MonitoringTool (Running) - Microsoft Visual Studio---New notification---------PersonalAnalytics: Retrospection & Insights" };




            List<List<string>> data = new List<List<string>>();
            data.Add(a);
            data.Add(b);
            data.Add(c);
            data.Add(d);
            data.Add(e);
            data.Add(f);
            data.Add(g);
            data.Add(h);
            data.Add(i);
            data.Add(j);
            data.Add(k);
            data.Add(l);
            //data.Add(temp1);

            BagOfWords bag = new BagOfWords(data,0.6);
            


            foreach(Dictionary<string, int> dict in bag.getBag())
            {
                
                foreach (KeyValuePair<string, int> entry in dict)
                {

                    Console.WriteLine(entry.Key + ": " + entry.Value);
                }
                Console.WriteLine("----------------");
            }

            Console.ReadLine();
        }
    }
}
