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
            List<string> temp = new List<string> { "testing is a great fun deal tree", "test1 hi nope recursion badd children fishing" };
            
           

            DataCleaner cleaner = new DataCleaner();
            cleaner.clean(temp);
            List<List<string>> test = cleaner.clean(temp);

            foreach(List<string> list in test)
            {
                foreach(string x in list)
                {
                    Console.WriteLine(x);
                    
                }
            }

            Console.ReadLine();
        }
    }
}
