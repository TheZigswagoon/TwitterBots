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

namespace TwennyWunBot
{
    class Program
    {
        //Phrases you want the bot to reply with
        static string[] phrases = { "You stoopid!", "You stoopid!", "You stoopid!", "You stoopid!" };
        static void Main(string[] args)
        {
            //Opens up the credential file. Its where your OAuth tokens are stored.
            StreamReader cred = new StreamReader("Cred.txt");
            string token1 = cred.ReadLine();
            string token2 = cred.ReadLine();
            cred.Close();
            //If your tokens are not there
            if (token2 == null)
            {
                //Sets this applications creds
                var applicationCredentials = CredentialsCreator.GenerateApplicationCredentials("", "");
                var url = CredentialsCreator.GetAuthorizationURL(applicationCredentials);
                //Sets the required url for authorization. Use the given url sign in with your account and enter the key given
                //into the console.
                Console.Write(url);
                Debug.Write(url);
                string captia = Console.ReadLine();
                //Gets your tokens from the code given
                var creds = CredentialsCreator.GetCredentialsFromVerifierCode(captia, applicationCredentials);
                StreamWriter f = new StreamWriter("Cred.txt");
                //Writes the codes to the cred document
                f.WriteLine(creds.AccessToken);
                f.WriteLine(creds.AccessTokenSecret);
                f.Close();
            }

            else
            {
                try
                {
                    //More or less logging in. Last 2 are the applications keys
                    TwitterCredentials.SetCredentials(token1, token2, "", "");
                }
                catch (Exception)
                {
                    Console.WriteLine("Error setting credentials!");
                }
                //As a note 1000 is 1 second.
                
                // Hook up the Elapsed event for the timer.
                Timer aTimer = new System.Timers.Timer(1000 * 60);
                aTimer.Elapsed += OnTimedEvent;
                aTimer.Enabled = true;
                Timer MentionsTimer = new System.Timers.Timer(1000 * 10);
                MentionsTimer.Elapsed += Ment;
                MentionsTimer.Enabled = true; 

                Console.WriteLine("Started!");
                //Lol if you wanna end the program. Just enter anything into the console. BOP its gone.
                Console.ReadLine();
            }
        }

            private static void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                int p = 0;
                Boolean didbreak = false;
                //Returns an array of tweets containing those words.
                var tweets = Search.SearchTweets("9 + 10 is 21");

                for (int i = 0; i < tweets.Count(); i++)
                {
                    //We like to respect twitter rate limits dont we? Lol idk the first thing about them.
                    System.Threading.Thread.Sleep(1000);
                    //Generating random responses tbt phrases.Count() would prob work too.
                    Random ran = new Random(3);
                    Random random = new Random();
                    int randomNumber = random.Next(0, 3);
                    //Spam is more or less a place holder for a tweet id.
                    string spam = "";
                    StreamReader ids = new StreamReader("ID.txt");
                    while (!ids.EndOfStream)
                    {
                        spam = ids.ReadLine();
                        //IMPORTANT THO. Makes sure it isnt replying to the same tweet!
                        if (tweets.ElementAt(i).Id.ToString() == spam)
                        {
                            didbreak = true;
                            break;
                        }
                        p++;
                    }
                    ids.Close();
                    if (didbreak == false)
                    {
                        Tweet.PublishTweetInReplyTo("@" + tweets.ElementAt(i).Creator.ScreenName + " " + phrases.ElementAt(randomNumber), tweets.ElementAt(i).Id);
                        Console.WriteLine("@" + tweets.ElementAt(i).Creator.ScreenName + " " + phrases.ElementAt(randomNumber));
                        using (StreamWriter sw = File.AppendText("ID.txt"))
                        {
                            sw.WriteLine(tweets.ElementAt(i).Id);
                            sw.Close();
                        }
                    }
                }
            }

            private static void Ment(Object source, ElapsedEventArgs e)
            {
                int p = 0;
                Boolean didbreak = false;
                //Returns an array of tweets containing those words. In this case I am getting my mentions in a hacky way.
                //The official method may be broke? Not sure. It turns a null pointer.
                var mentions = Search.SearchTweets("@twennywunbot ");

                for (int i = 0; i < mentions.Count(); i++)
                {
                    //Time to see what kind of tweet it is. Do we reply to it and favorite or just favorite?
                    if ((mentions.ElementAt(i).Text.ToLower().Contains("9")
                        && mentions.ElementAt(i).Text.ToLower().Contains("10")
                        && mentions.ElementAt(i).Text.ToLower().Contains("+")
                        && (mentions.ElementAt(i).Text.ToLower().Contains("what")
                        || mentions.ElementAt(i).Text.ToLower().Contains("whats")
                        || mentions.ElementAt(i).Text.ToLower().Contains("what's")
                        || mentions.ElementAt(i).Text.ToLower().Contains("what is")))
                        || mentions.ElementAt(i).Text.ToLower().Contains("you stupid")
                        || mentions.ElementAt(i).Text.ToLower().Contains("u stupid")
                        )
                    {
                        //Nigga we made it we reply to it.
                        string spam = "";
                        StreamReader ids = new StreamReader("ID.txt");
                        while (!ids.EndOfStream)
                        {
                            spam = ids.ReadLine();
                            //IMPORTANT THO. Makes sure it isnt replying to the same tweet!
                            if (mentions.ElementAt(i).Id.ToString() == spam)
                            {
                                didbreak = true;
                                break;
                            }
                            p++;
                        }
                        ids.Close();
                        if (didbreak == false)
                        {
                            //Ok so we here now. Now to figure out which condition it exactly was...
                            if ((mentions.ElementAt(i).Text.ToLower().Contains("9")
                        && mentions.ElementAt(i).Text.ToLower().Contains("10")
                        && mentions.ElementAt(i).Text.ToLower().Contains("+")
                        && (mentions.ElementAt(i).Text.ToLower().Contains("what")
                        || mentions.ElementAt(i).Text.ToLower().Contains("whats")
                        || mentions.ElementAt(i).Text.ToLower().Contains("what's")
                        || mentions.ElementAt(i).Text.ToLower().Contains("what is"))))
                            {
                                //The question is relevant to what is 9+10. Time to respond 21!
                                mentions.ElementAt(i).Favourite();
                                Tweet.PublishTweetInReplyTo("@" + mentions.ElementAt(i).Creator.ScreenName + " " + "21", mentions.ElementAt(i).Id);
                                Console.WriteLine("@" + mentions.ElementAt(i).Creator.ScreenName + " " + "21");
                            }
                            else
                            {
                                //Stupid? Yes we are! Or are we?
                                mentions.ElementAt(i).Favourite();
                                Tweet.PublishTweetInReplyTo("@" + mentions.ElementAt(i).Creator.ScreenName + " " + "No I not", mentions.ElementAt(i).Id);
                                Console.WriteLine("@" + mentions.ElementAt(i).Creator.ScreenName + " " + "No I not");
                            }
                            using (StreamWriter sw = File.AppendText("ID.txt"))
                            {
                                sw.WriteLine(mentions.ElementAt(i).Id);
                                sw.Close();
                            }
                        }
                    }
                    else
                    {
                        //Cause we are nice people and favorite everything!
                        mentions.ElementAt(i).Favourite();
                    }
                }
            }
    }
}
