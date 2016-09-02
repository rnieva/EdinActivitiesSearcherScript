using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ActivitiEdinSearchScript
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://www.joininedinburgh.org/?q=&at=&ns="; // Any Activity type
            string url2 = "https://www.joininedinburgh.org/?q=english&at=46&ns="; // Keyword = english,
            var client = new WebClient();
            string webContent = client.DownloadString(url2);
            Console.WriteLine(webContent);
            Console.ReadKey();
        }
    }
}
