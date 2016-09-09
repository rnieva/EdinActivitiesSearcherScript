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
            string urlHost = "https://www.joininedinburgh.org/";
            string url3= urlHost + "?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year="; // Keyword = english,
            string url = urlHost + "?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=morning"; // Keywords = english, morning
            Console.WriteLine("KeyWords: " + GetNumberOfKeyWords(url));
            string webContent = "";
            webContent = GetWebContent(url, webContent);
            // Regular Expression
            Console.WriteLine(GetNumberOfResults(webContent));
            string olContent = "";
            olContent = GetContentResultsOl(webContent, olContent);
            ArrayList listMatchesDiv = new ArrayList();
            listMatchesDiv = GetContentResultsDiv(olContent, listMatchesDiv);
            List<string> listLinkResults = new List<string>();
            listLinkResults = GetLinksResults(listMatchesDiv, listLinkResults);
            ArrayList listLinks = new ArrayList();
            listLinks = GetLinksMoreResults(listMatchesDiv, listLinks); 
            ArrayList listItems = new ArrayList();
            listItems = GetListMatchesClear(listMatchesDiv, listItems);
            int i = 0;
            if (listLinks != null) // If there is more pages with results (10 per page)
            {
                for (i=0;i<listLinks.Count-1;i++)
                {
                    url = urlHost + listLinks[i];
                    webContent = GetWebContent(url, webContent);
                    olContent = GetContentResultsOl(webContent, olContent);
                    listMatchesDiv = GetContentResultsDiv(olContent, listMatchesDiv);
                    listLinkResults = GetLinksResults(listMatchesDiv, listLinkResults);
                    listItems = GetListMatchesClear(listMatchesDiv, listItems);
                }
            }
            Console.WriteLine(ShowInfo(listItems));
           
            Console.ReadKey();
            // TODO: build a HTML page
            // TODO: void to send by email
        }

        static string GetNumberOfKeyWords(string url) //search the keywords in the URL
        {
            string keyWordsString = "";
            ArrayList keyWords = new ArrayList();
            string h1Regex = "(\\?|\\&)([^=]+)\\=([^ &]+)";
            MatchCollection mc = Regex.Matches(url, h1Regex, RegexOptions.Singleline);
            foreach (Match m in mc)
            {
                keyWords.Add(m.Groups[3].Value);
                if (m.Groups[2].Value != "at")
                {
                    keyWordsString = keyWordsString + m.Groups[3].Value + " ";
                }
            }
            return keyWordsString;
        }

        static string GetWebContent(string url, string webContent) // extract code page url
        {
            var client = new WebClient();
            webContent = client.DownloadString(url);
            return webContent;
        }

        static string GetNumberOfResults(string webContent) // find the number of results
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

        static string GetContentResultsOl(string webContent, string olContent)  //First filter 
        {
            String olRegex = "<ol class=\"search-results\">(?<content>.*)<div id=\"column\">"; //read from class="search-result" until id="column", the final the second ol
            MatchCollection mc2 = Regex.Matches(webContent, olRegex, RegexOptions.Singleline);
            foreach (Match m in mc2)
            {
                olContent = (m.Groups["content"].Value);
            }
            return olContent;
        }

        static ArrayList GetContentResultsDiv(string olContent, ArrayList listMatchesDiv) // Second filter
        {
            listMatchesDiv.Clear(); // We need clear this Array, Maybe there will be a next page
            String divRegex = "<div[^>]*?>(.*?)</div>"; // content within the divs within olContent
            MatchCollection mc3 = Regex.Matches(olContent, divRegex, RegexOptions.Singleline);

            foreach (Match m in mc3)
            {
                listMatchesDiv.Add(m.Groups[1].Value);
            }
            return listMatchesDiv;
        }

        static List<string> GetLinksResults(ArrayList listMatchesDiv, List<string> listLinkResults) //Get the links of each result
        {
            string href = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            int i = 0;
            for (i = 0; i < listMatchesDiv.Count - 1; i++)
            {
                MatchCollection moreResults2 = Regex.Matches(listMatchesDiv[i].ToString(), href, RegexOptions.Singleline);
                foreach (Match m in moreResults2)
                {
                    listLinkResults.Add("https://www.joininedinburgh.org" + m.Groups[1].Value);
                }
            }
            listLinkResults = listLinkResults.Distinct().ToList();
            return listLinkResults;
        }

        static ArrayList GetLinksMoreResults(ArrayList listMatchesDiv, ArrayList listLinks) // Get the links the next pages, only if there is more than 10 results
        {
            string href = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            MatchCollection moreResults = Regex.Matches(listMatchesDiv[listMatchesDiv.Count - 1].ToString(), href, RegexOptions.Singleline);
            foreach (Match m in moreResults)
            {
                listLinks.Add(m.Groups[1].Value);
            }
            return listLinks;
        }

        static ArrayList GetListMatchesClear(ArrayList listMatchesDiv, ArrayList listItems) //Clear <p> and <ul> tags
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
            for (i = 0; i < listMatchesClear.Count; i++)  //Clear spaces-tab-html content
            {
                string result = (Regex.Replace(listMatchesClear[i].ToString(), @"<[^>]*>", String.Empty).Trim());
                listItems.Add(Regex.Replace(result, @"\t|\n|\r\s+", ""));
            }
            return listItems;
        }

        static StringBuilder ShowInfo(ArrayList listItems) //Return in a StringBuilder the info
        {
            int i = 0;
            int index = 1;
            StringBuilder infoTotal = new StringBuilder();
            for (i = 0; i < listItems.Count - 1; i++)
            {
                if (i == 0)
                {
                    infoTotal.AppendLine((index++).ToString() + "- " + listItems[i]);
                }
                if (listItems[i].ToString() == "ENDITEM")
                {
                    infoTotal.AppendLine((index++).ToString() + "- " + listItems[i + 1]);
                }
                if ((listItems[i].ToString() == "View details") && (listItems[i - 1].ToString().Substring(0, 4) == "Next"))
                {
                    infoTotal.AppendLine("\t- " + listItems[i - 1]);
                }
                if (listItems[i].ToString() == "View details")
                {
                    string temp = listItems[i + 1].ToString();
                    infoTotal.AppendLine("\t- " + listItems[i + 1].ToString().Substring(0, temp.LastIndexOf("Edinburgh") + 9));
                    infoTotal.AppendLine();
                }
            }
            return infoTotal;
        }
    }
}
