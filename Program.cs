using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ActivitiEdinSearchScript
{
    class Program
    {
        static void Main(string[] args)     //other way htmlagilitypack
        {
            string url = "https://www.joininedinburgh.org/?q=&at=&ns="; // Any Activity type
            string url2 = "https://www.joininedinburgh.org/?q=english&at=46&ns="; // Keyword = english,
            var client = new WebClient();
            string webContent = client.DownloadString(url2);
            // Regular Expression
            String h1Regex = "<h1[^>]*?>(?<TagText>.*?)</h1>";
            MatchCollection mc = Regex.Matches(webContent, h1Regex, RegexOptions.Singleline);
            foreach (Match m in mc)
            {
                Console.WriteLine(m.Groups["TagText"].Value);
            }
            

            //Console.WriteLine(mc);
            Console.ReadKey();
        }
       

       

    }
}
