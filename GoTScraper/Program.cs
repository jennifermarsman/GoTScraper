using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace GoTScraper
{
    class Program
    {
        static string outputDirectory = "";

        /// <summary>
        /// This is a program that scrapes the website https://www.springfieldspringfield.co.uk/episode_scripts.php?tv-show=game-of-thrones
        /// to download the Game of Thrones television scripts.  
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Format: https://www.springfieldspringfield.co.uk/view_episode_scripts.php?tv-show=game-of-thrones&episode=s01e01

            InitializeOutputDirectory();
            DownloadGoTScripts();
        }

        private static void InitializeOutputDirectory()
        {
            // Set a variable to the My Documents path.
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Set the output directory.  If you change this, make sure it ends with a "\".  
            outputDirectory = myDocumentsPath + @"\Data\GoT\";

            // Check that directory exists and create it if it doesn't
            if (!Directory.Exists(outputDirectory))
            {
                DirectoryInfo dirInfo = Directory.CreateDirectory(outputDirectory);
                Console.WriteLine("Created directory: " + dirInfo.FullName);
            }
        }

        private static void DownloadGoTScripts()
        {
            string address = "https://www.springfieldspringfield.co.uk/view_episode_scripts.php?tv-show=game-of-thrones&episode=";
            HttpClient client = new HttpClient();

            for (int season = 1; season < 8; season++)
            {
                for (int episode = 1; episode <= 10; episode++)
                {
                    //Uri uri = new Uri(address + "s" + season.ToString("00") + "e" + episode.ToString("00"));
                    //client.BaseAddress = uri;
                    string uri = address + "s" + season.ToString("00") + "e" + episode.ToString("00");
                    //string response = await client.GetStringAsync(uri);
                    //ParseResponse(response);    // TODO: need to do this sync?  

                    // TODO: break conditions for seasons that don't have 10 episodes

                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load(uri);
                    ParseResponse(doc);
                }
            }
        }

        private static void ParseResponse(HtmlDocument doc)
        {
            // Put relevant info from HTML
            var mainContentLeftNode = doc.DocumentNode.ChildNodes["html"].ChildNodes["body"].ChildNodes[1].ChildNodes[7].ChildNodes[7].ChildNodes[3];
            string filename = mainContentLeftNode.ChildNodes["h1"].InnerText.Trim();
            string episodeTitle = mainContentLeftNode.ChildNodes["h3"].InnerText.Trim();
            string episodeContent = mainContentLeftNode.ChildNodes[7].ChildNodes[1].InnerHtml.Trim();

            // Turn the breaks into new lines in the output text
            string[] splitOn = { "<br>" };
            string[] lines = episodeContent.Split(splitOn, StringSplitOptions.RemoveEmptyEntries);

            // Write to file
            Console.WriteLine("Writing " + filename + ": " + episodeTitle);
            using (StreamWriter outputFile = new StreamWriter(outputDirectory + filename + ".txt", false))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line.Trim());
                }
            }

            /*
            // With LINQ	
            var nodes = doc.DocumentNode.Descendants("html")
            .Select(y => y.Descendants()
            .Where(x => x.Attributes["class"].Value == "main-content-left"))
            .ToList();

            // html, body, div[1], div[3], div[2]
            // With XPath	
            var value = doc.DocumentNode
            .SelectNodes(@"/html[1]/body[1]")
            .First()
            .Attributes["value"].Value;
            */
        }
    }
}
