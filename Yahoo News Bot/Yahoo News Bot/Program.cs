using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Timers;
using System.IO;
using System.Diagnostics;
using Tweetinvi;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Controllers;
using Tweetinvi.Core.Interfaces.DTO;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Interfaces.Models.Parameters;
using Tweetinvi.Core.Interfaces.oAuth;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Json;


namespace Yahoo_News_Bot
{
    class Program
    {
       

       static void Main(string[] args)
       {

           StreamReader cred = new StreamReader("Cred.txt");
            string token1 = cred.ReadLine();
            string token2 = cred.ReadLine();
            cred.Close();
            if (token2 == null)
            {
                var applicationCredentials = CredentialsCreator.GenerateApplicationCredentials("", "");
                var url = CredentialsCreator.GetAuthorizationURL(applicationCredentials);
                Console.Write(url);
                Debug.Write(url);
                string captia = Console.ReadLine();
                var creds = CredentialsCreator.GetCredentialsFromVerifierCode(captia, applicationCredentials);
                StreamWriter f = new StreamWriter("Cred.txt");
                f.WriteLine(creds.AccessToken);
                f.WriteLine(creds.AccessTokenSecret);
                f.Close();
            }

            else
            {
                try
                {
                    TwitterCredentials.SetCredentials(token1, token2, "", "");
                }
                catch (Exception)
                {
                    Console.WriteLine("Error setting credentials!");
                }
                //Console.WriteLine(word);

                
                Timer timer = new Timer(1000 * 60 * 5);
                timer.Elapsed += OnTimedEvent;
                timer.Start();
                Console.WriteLine("Started!");
                Console.ReadLine();
            }
       }

       private static void OnTimedEvent(Object source, ElapsedEventArgs e)
       {
           XmlReader responseReader = XmlReader.Create("http://news.yahoo.com/rss/");
           SyndicationFeed feed = SyndicationFeed.Load(responseReader);
           string id = feed.Items.ElementAt(0).Id;
           StreamReader ids = new StreamReader("ID.txt");
           string theid = ids.ReadLine();
           ids.Close();
           if(!theid.Equals(id) || theid.Equals(null))
           {
                string title = feed.Items.ElementAt(0).Title.Text;
                string url = feed.Items.ElementAt(0).Links.ElementAt(0).Uri.ToString();

                Console.Write(title + " : " + url + "\n");
                Tweet.PublishTweet(title + " " + url);
                StreamWriter write = new StreamWriter("ID.txt");
                write.WriteLine(id);
                write.Close();
           }
       }
    }
}
