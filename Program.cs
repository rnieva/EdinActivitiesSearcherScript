using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net.Mail;
using System.Xml.Linq;

namespace ActivitiEdinSearchScript
{
    class Program
    {
        static void Main(string[] args)     // TODO: other way htmlagilitypack
        {
            string urlHost = "https://www.joininedinburgh.org/";
            //string url3= urlHost + "?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year="; // Keyword = english,
            //string url2 = urlHost + "?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=morning"; // Keywords = english, morning
            //string url = urlHost + "?q=english&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&ns=on";
            string url = ConfigKeys(urlHost);

            Console.WriteLine("KeyWords: " + GetNumberOfKeyWords(url));
            StringBuilder infoTotal = SearchResults(url, urlHost);
            Console.WriteLine(infoTotal);
            HtmlGenerater(infoTotal);  //TODO: build a HTML page
            sendEmail(infoTotal);
            Console.ReadKey();
        }

        static string ConfigKeys(string urlHost)
        {
            string url = "?q=&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=";
            string keyWord = "english";
            Console.WriteLine("Keyword:");
            keyWord = Console.ReadLine();
            string acceptingStudents = "on";
            Console.WriteLine("Accepting students(Y/N)");
            ConsoleKeyInfo keyPressed = Console.ReadKey();
            if ((keyPressed.KeyChar == 'Y') || (keyPressed.KeyChar == 'y'))
            {
                acceptingStudents = "on";
            }
            else
            {
                acceptingStudents = "";
            }
            Console.WriteLine("Time of day: Morning(m), Afternoon(a) or Evening(e)");
            keyPressed = Console.ReadKey();
            switch (keyPressed.KeyChar)
            {
                case 'm':
                    url = "?q=" + keyWord + "&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=morning&ns="+ acceptingStudents;
                    break;
                case 'a':
                    url = "?q=" + keyWord + "&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=afternoon&ns="+ acceptingStudents;
                    break;
                case 'e':
                    url = "?q=" + keyWord + "&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&t=evening&ns=" +acceptingStudents;
                    break;
                default:
                    url = "?q=" + keyWord + "&at=46&a=&distance=&pc=&location=&ds_month_year=&de_month_year=&ns=" + acceptingStudents;
                    break;
            }
            url = urlHost + url;
            Console.WriteLine(url + "\n");
            return url;
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

        static StringBuilder SearchResults(string url, string urlHost) // return a StringBuilder with final result
        {   // Regular Expression
            string webContent = "";
            webContent = GetWebContent(url, webContent);
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
                for (i = 0; i < listLinks.Count - 1; i++)
                {
                    url = urlHost + listLinks[i];
                    webContent = GetWebContent(url, webContent);
                    olContent = GetContentResultsOl(webContent, olContent);
                    listMatchesDiv = GetContentResultsDiv(olContent, listMatchesDiv);
                    listLinkResults = GetLinksResults(listMatchesDiv, listLinkResults);
                    listItems = GetListMatchesClear(listMatchesDiv, listItems);
                }
            }
            StringBuilder infoTotal = PrepareInfo(listItems);
            return infoTotal;
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
            try {
                MatchCollection moreResults = Regex.Matches(listMatchesDiv[listMatchesDiv.Count - 1].ToString(), href, RegexOptions.Singleline);
                foreach (Match m in moreResults)
                {
                    listLinks.Add(m.Groups[1].Value);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error to sent email: " + ex.Message);
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

        static StringBuilder PrepareInfo(ArrayList listItems) //Return in a StringBuilder the info
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
                    infoTotal.AppendLine ();
                }
            }
            return infoTotal;
        }

        static void HtmlGenerater(StringBuilder infoTotal)
        {
            var html = new XElement("html",
                        new XElement("head",
                            new XElement("title", "My Page")
                        ),
                        new XElement("body",
                            "this is some text"
                        )
                    );
            int testBreak = 1;
        }

        static void sendEmail(StringBuilder infoTotal)
        {
            string emailrecipient = "roberto.nieva@gmail.com";
            MailMessage email = new MailMessage();
            email.To.Add(new MailAddress(emailrecipient));
            email.From = new MailAddress("nieva_roberto@hotmail.com", "EdinSearchScript");
            email.Subject = " Info ( " + DateTime.Now.ToString("dd / MMM / yyy hh:mm:ss") + " ) ";
            email.Body = "Results:  <br>";
            email.Body = infoTotal.ToString();
            email.IsBodyHtml = false; // If =true, You must to add </Br> in the body
            email.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.live.com";
            smtp.Port = 25;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("nieva_roberto@hotmail.com", ""); // xxx, pass email
            try
            {
                smtp.Send(email);
                email.Dispose();
                Console.WriteLine("Send complete");
                System.Diagnostics.Debug.WriteLine("Email sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail sending Email");
                System.Diagnostics.Debug.WriteLine("Error to sent email: " + ex.Message);
            }
            
        }
    }
}
