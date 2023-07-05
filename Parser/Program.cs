using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using HtmlAgilityPack;

public class Program
{
    public static void Main()
    {
        string url = "https://www.ardic.com/products";

        // Send a GET request to the website
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
                            uniqueUrls.Add(subUrl);
                            Console.WriteLine("Fetching URL: " + subUrl);
                            LoadChildren(subUrl);
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

    public static void LoadChildren(string urlPart)
    {
        string url = $"https://www.ardic.com/{urlPart}";

        // Send a GET request to the website
        WebClient client = new WebClient();
        string htmlContent = client.DownloadString(url);

        // Load the HTML content into HtmlDocument
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        // Find the div element with id="content" and class="content" and role="main"
        HtmlNode contentDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='content' and @class='content' and @role='main']");

        if (contentDiv != null)
        {
            // Find all child elements with class "wpb_column vc_column_container vc_col-sm-4"
            HtmlNodeCollection childNodes = contentDiv.SelectNodes(".//div[contains(@class, 'wpb_column vc_column_container vc_col-sm-4')]");

            if (childNodes != null)
            {
                // Get the last part of the URL as the directory name
                string directoryName = GetLastUrlPart(url);

                // Create the directory for saving images if it does not exist
                string saveDirectory = directoryName;
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                // Process each child element
                foreach (HtmlNode childNode in childNodes)
                {
                    // Find the image tag within the child element
                    HtmlNode imageNode = childNode.SelectSingleNode(".//img");
                    if (imageNode != null)
                    {
                        string imageUrl = imageNode.GetAttributeValue("src", "");
                        string fileName = GetValidFileName(HttpUtility.HtmlDecode(childNode.InnerText.Trim())) + ".jpg";
                        string savePath = Path.Combine(saveDirectory, fileName);

                        try
                        {
                            using (WebClient imageClient = new WebClient())
                            {
                                imageClient.DownloadFile(imageUrl, savePath);
                                Console.WriteLine("Downloaded: " + savePath);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error downloading image: " + e.Message);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No child elements with class 'wpb_column vc_column_container vc_col-sm-4' found.");
            }
        }
        else
        {
            Console.WriteLine("Div element with id='content' and class='content' and role='main' not found.");
        }
    }

    private static string GetLastUrlPart(string url)
    {
        if (url.EndsWith("/"))
            url = url.Substring(0, url.Length - 1);
        Uri uri = new Uri(url);
        string[] parts = uri.AbsolutePath.Split('/');
        return parts[parts.Length - 1];
    }

    private static string GetValidFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
    }
}
