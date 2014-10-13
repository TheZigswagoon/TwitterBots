using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace WOMBO_COMBO_BOT
{
    class Program
    {
        
       static string[] phrases = { "THAT AIN'T FALCO!", "HAPPY FEET!", "WHERE YOU AT WHERE YOU AT?!", "GET YO ASS WHOOPED"};
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




                //Timer aTimer = new System.Timers.Timer(1000 * 60 * 10);
                Timer aTimer = new System.Timers.Timer(1000 * 60);
                // Hook up the Elapsed event for the timer. 
                Console.WriteLine("Started!");
                aTimer.Elapsed += OnTimedEvent;
                aTimer.Enabled = true;
                Console.ReadLine();
            }
        }

            private static void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                int p = 0;
                Boolean didbreak = false;
                var tweets = Search.SearchTweets("wombo combo");

                for (int i = 0; i < tweets.Count(); i++)
                {

                    Random ran = new Random(3);
                    Random random = new Random();
                    int randomNumber = random.Next(0, 3);

                    string spam = "";
                    StreamReader ids = new StreamReader("ID.txt");
                    while (!ids.EndOfStream)
                    {

                        spam = ids.ReadLine();
                        if (tweets.ElementAt(i).Id.ToString() == spam)
                        {
                            didbreak = true;
                            break;
                        }


                        p++;
                    }
                    ids.Close();
                    if (!didbreak && tweets.ElementAt(i).Creator.ScreenName != "WomboComboBot")
                    {
                        if (tweets.ElementAt(i).Creator.ScreenName.Equals("OfficialViews"))
                        {
                            Tweet.PublishTweetInReplyTo("@OfficialViews YOU SUCK", tweets.ElementAt(i));
                            Console.WriteLine("@OfficialViews YOU SUCK", tweets.ElementAt(i));
                        }
                        else
                        {
                            Tweet.PublishTweetInReplyTo("@" + tweets.ElementAt(i).Creator.ScreenName + " " + phrases.ElementAt(randomNumber), tweets.ElementAt(i).Id);
                            Console.WriteLine("@" + tweets.ElementAt(i).Creator.ScreenName + " " + phrases.ElementAt(randomNumber));
                        }
                       // StreamWriter idz = new StreamWriter("ID.txt");
                        using (StreamWriter sw = File.AppendText("ID.txt"))
                        {
                            sw.WriteLine(tweets.ElementAt(i).Id);
                            sw.Close();
                        }	
                       // idz.WriteLine(tweets.ElementAt(i).Id + "\n");
                        //idz.Close();

                    }

                }
            }


    }
}
