using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace ActivitiEdinSearchScript
{
    class Program
    {
        static void Main(string[] args)     // TODO: other way htmlagilitypack
        {
            string url2 = "https://www.joininedinburgh.org/?q=english&at=46&ns="; // Keyword = english,
            string url = "https://www.joininedinburgh.org/?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=morning"; // Keywords = english, morning
            string webContent = "";
            webContent = GetWebContent(url, webContent);
            // Regular Expression
            Console.WriteLine(ShowNumberOfResults(webContent));
            string olContent = "";
            olContent = GetContentResultsOl(webContent, olContent);
            ArrayList listMatchesDiv = new ArrayList();
            listMatchesDiv = GetContentResultsDiv(olContent, listMatchesDiv);
            ArrayList listLinks = new ArrayList();
            listLinks = GetLinksMoreResuts(listMatchesDiv, listLinks); //TODO; look for first the number of pages
            ArrayList listItems = new ArrayList();
            listItems = GetListMatchesClear(listMatchesDiv, listItems);
            int i = 0;
            if (listLinks != null)
            {
                for (i=0;i<listLinks.Count-1;i++)
                {
                    url = url + listLinks[i];
                    webContent = GetWebContent(url, webContent);
                    olContent = GetContentResultsOl(webContent, olContent);
                    listMatchesDiv = GetContentResultsDiv(olContent, listMatchesDiv);
                    listItems = GetListMatchesClear(listMatchesDiv, listItems);
                }
            }
            // TODO: void to send by email
            i = 0;
            int index = 1;
            for (i =0;i<listItems.Count-1;i++)
            {
                if (i == 0)
                {
                    Console.WriteLine((index++).ToString() + "- " + listItems[i]);
                }
                if (listItems[i] == "ENDITEM")
                {
                    Console.WriteLine((index++).ToString() + "- " + listItems[i+1]);
                }
            }
            Console.ReadKey();
        }

        static string GetWebContent(string url, string webContent)
        {
            var client = new WebClient();
            webContent = client.DownloadString(url);
            return webContent;
        }

        static string ShowNumberOfResults(string webContent)
        {
            string numberOfResult = "";
            string h1Regex = "<h1[^>]*?>(?<TagText>.*?)</h1>";
            MatchCollection mc = Regex.Matches(webContent, h1Regex, RegexOptions.Singleline);
            foreach (Match m in mc)
            {
                numberOfResult = (m.Groups["TagText"].Value);
            }
            return numberOfResult;
        }

        static string GetContentResultsOl(string webContent, string olContent)
        {
            String olRegex = "<ol class=\"search-results\">(?<content>.*)<div id=\"column\">"; //read from class="search-result" until id="column", the final the second ol
            MatchCollection mc2 = Regex.Matches(webContent, olRegex, RegexOptions.Singleline);
            foreach (Match m in mc2)
            {
                olContent = (m.Groups["content"].Value);
            }
            return olContent;
        }

        static ArrayList GetContentResultsDiv(string olContent, ArrayList listMatchesDiv)
        {
            listMatchesDiv.Clear(); // We need clear this Array, Maybe there will be a next page
            String divRegex = "<div[^>]*?>(.*?)</div>"; // content within the divs
            MatchCollection mc3 = Regex.Matches(olContent, divRegex, RegexOptions.Singleline);

            foreach (Match m in mc3)
            {
                listMatchesDiv.Add(m.Groups[1].Value);
            }
            return listMatchesDiv;
        }

        static ArrayList GetLinksMoreResuts(ArrayList listMatchesDiv, ArrayList listLinks)
        {
            string href = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            MatchCollection moreResults = Regex.Matches(listMatchesDiv[listMatchesDiv.Count - 1].ToString(), href, RegexOptions.Singleline);
            foreach (Match m in moreResults)
            {
                listLinks.Add(m.Groups[1].Value);
            }
            return listLinks;
        }

        static ArrayList GetListMatchesClear(ArrayList listMatchesDiv, ArrayList listItems)
        {
            ArrayList listMatchesClear = new ArrayList();
            String itemRegex = "<p[^>]*?>(.*?)</p>";
            String itemRegex2 = "<ul[^>]*?>(.*?)</ul>";
            int i = 0;
            for (i = 0; i < listMatchesDiv.Count; i++)
            {
                MatchCollection mc4 = Regex.Matches(listMatchesDiv[i].ToString(), itemRegex, RegexOptions.Singleline);
                foreach (Match m in mc4)
                {
                    listMatchesClear.Add(m.Groups[1].Value);
                }

                MatchCollection mc5 = Regex.Matches(listMatchesDiv[i].ToString(), itemRegex2, RegexOptions.Singleline);
                foreach (Match m in mc5)
                {
                    listMatchesClear.Add(m.Groups[1].Value);
                    listMatchesClear.Add("ENDITEM");
                }

            }
            i = 0;
            for (i = 0; i < listMatchesClear.Count; i++)
            {
                string result = (Regex.Replace(listMatchesClear[i].ToString(), @"<[^>]*>", String.Empty));
                listItems.Add(Regex.Replace(result, @"\t|\n|\r", ""));
            }
            return listItems;
        }

    }
}
