using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace BorsanParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FetchCategory("https://www.borsan.com.tr/bakir-iletkenli-guc-tesisat-kablolari/");
            FetchCategory("https://www.borsan.com.tr/aluminyum-iletkenli-guc-ve-tesisat-kablolari/");
            FetchCategory("https://www.borsan.com.tr/zayif-akim-ve-ozel-tasarim-kablolar/");
            FetchCategory("https://www.borsan.com.tr/en/telecom-and-special-design-cables/");
            Console.WriteLine("The download is complete");
            Console.WriteLine("Press Enter to exit ..");
            Console.ReadLine();
        }

        static void FetchCategory(string url)
        {
            WebClient client = new WebClient();
            string htmlContent = client.DownloadString(url);

            // Load the HTML content into HtmlDocument
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // Find the div element with class="content"
            //var section = htmlDocument.DocumentNode.SelectSingleNode("vc_row wpb_row vc_row-fluid")
            HtmlNode contentDiv = htmlDocument.DocumentNode.SelectSingleNode("//section[contains(@class, 'wpb_row')]");

            if (contentDiv != null)
            {
                // Parse HTML using HtmlAgilityPack
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(contentDiv.InnerHtml);

                // Modify the XPath expression according to the structure of the webpage
                var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]").ToList();

                linkNodes.ForEach(x =>
                {
                    fetchSubItems(x.Attributes[0].Value);
                });
            } 
            else
            {
                Console.WriteLine("Div element with class 'content' not found.");
            }
        }

        static void fetchSubItems(string url)
        {
            try
            {
                WebClient client = new WebClient();
                string htmlContent = client.DownloadString(url);

                // Load the HTML content into HtmlDocument
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);

                // Find the div element with class="content"
                //var section = htmlDocument.DocumentNode.SelectSingleNode("vc_row wpb_row vc_row-fluid")
                HtmlNode contentDiv = htmlDocument.DocumentNode.SelectSingleNode("//section[contains(@class, 'wpb_row')]");

                if (contentDiv != null)
                {
                    // Parse HTML using HtmlAgilityPack
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(contentDiv.InnerHtml);

                    // Modify the XPath expression according to the structure of the webpage
                    var linkNodes = doc.DocumentNode.SelectNodes("//img").ToList();

                    linkNodes.ForEach(x =>
                    {
                        // fetchSubItems(x.Attributes[0].Value);

                        var imgUrl = x.Attributes.FirstOrDefault(att => att.Name == "src").Value;
                        Download(imgUrl);
                    });
                }
                else
                {
                    Console.WriteLine("Div element with class 'content' not found.");
                }
            }
            catch { }
        }


        static void Download(string imgUrl)
        {
            try
            {
                if (!Directory.Exists("Images"))
                    Directory.CreateDirectory("Images");
                using (WebClient imageClient = new WebClient())
                {
                    var splits = imgUrl.Split('/');
                    var name = splits[splits.Length - 1];

                    string savePath = Path.Combine("Images", name);
                    imageClient.DownloadFile($"{imgUrl}", savePath);
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
