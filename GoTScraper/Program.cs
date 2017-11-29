using System;
using System.IO;
using System.Net.Http;
using HtmlAgilityPack;

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
                    // TODO: break conditions for seasons that don't have 10 episodes
                    // TODO: why isn't it pulling season 7?  Put a check for season == 7 and episode == 1 and skip that

                    string uri = address + "s" + season.ToString("00") + "e" + episode.ToString("00");
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load(uri);
                    ParseResponse(doc);
                }
            }
        }

        private static void ParseResponse(HtmlDocument doc)
        {
            // Put relevant info from HTML - I know this is ugly and hardcoded, but it will only be run once 
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
        }
    }
}
