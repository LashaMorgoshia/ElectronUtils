using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ParserCetinkayapano
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "https://cetinkayapano.com/wp-content/uploads/2021/08/";
            WebClient client = new WebClient();
            string htmlContent = client.DownloadString(url);

            // Load the HTML content into HtmlDocument
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // Find the div element with class="content"
            HtmlNode contentDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'content')]");

            if (contentDiv != null)
            {
                // Get all child elements
                HtmlNodeCollection childNodes = contentDiv.ChildNodes;

                // HashSet to store unique URLs
                HashSet<string> uniqueUrls = new HashSet<string>();

                // Process each child element
                foreach (HtmlNode childNode in childNodes)
                {
                    // Find the sub-children elements within the child element
                    HtmlNodeCollection subChildNodes = childNode.SelectNodes(".//a");

                    if (subChildNodes != null)
                    {
                        // Process each sub-child element
                        foreach (HtmlNode subChildNode in subChildNodes)
                        {
                            string subUrl = subChildNode.GetAttributeValue("href", "");
                            string content = HttpUtility.HtmlDecode(subChildNode.InnerText.Trim());

                            // Check if URL is unique
                            if (!uniqueUrls.Contains(subUrl))
                            {
                                if (subUrl.StartsWith("/wp-content"))
                                {
                                    Console.WriteLine("Fetching URL: " + subUrl);
                                    Download(subUrl, content);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Div element with class 'content' not found.");
            }
            Console.WriteLine("The download is complete");
            Console.WriteLine("Press Enter to exit ..");
            Console.ReadLine();
        }

        static void Download(string imgUrl, string name)
        {
            try
            {
                if (!Directory.Exists("Images"))
                    Directory.CreateDirectory("Images");
                using (WebClient imageClient = new WebClient())
                {
                    string savePath = Path.Combine("Images", name);
                    imageClient.DownloadFile($"https://cetinkayapano.com/{imgUrl}", savePath);
                    Console.WriteLine("Downloaded: " + name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error downloading image: " + e.Message);
            }
        }
    }
}