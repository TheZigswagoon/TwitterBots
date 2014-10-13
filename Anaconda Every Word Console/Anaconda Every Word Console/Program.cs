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
namespace Anaconda_Every_Word_Console
{
    class Program
    {
       static int i = 0;
       static int times = 0;
       static string word;
       static StreamReader words = new StreamReader("Words.txt");
       static StreamWriter progresssave;
        static void Main(string[] args)
        {
            StreamReader code = new StreamReader("Cred.txt");
            string code1 = code.ReadLine();
            string code2 = code.ReadLine();
            code.Close();
            if(code2 == null)
            {
                var applicationCredentials = CredentialsCreator.GenerateApplicationCredentials("", "");
                var url = CredentialsCreator.GetAuthorizationURL(applicationCredentials);
                Console.Write(url);
                Debug.Write(url);
                string captia = Console.ReadLine();
                var cred = CredentialsCreator.GetCredentialsFromVerifierCode(captia, applicationCredentials);
                StreamWriter f = new StreamWriter("Cred.txt");
                f.WriteLine(cred.AccessToken);
                f.WriteLine(cred.AccessTokenSecret);
                f.Close();
               
            }
            else
            {
                try
                {
                    TwitterCredentials.SetCredentials(code1, code2, "", "");
                }
                catch(Exception)
                {
                    Console.WriteLine("Error setting credentials!");
                }
                Timer aTimer = new System.Timers.Timer(30000);
                // Hook up the Elapsed event for the timer. 
                Console.WriteLine("Started!");
                aTimer.Elapsed += OnTimedEvent;
                aTimer.Enabled = true;
                Console.ReadLine();
            }
        }
        
            static string num ="";
           static int num1=0;
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
         {
             
            if (times == 0)
            {
                StreamReader progress = new StreamReader("Prog.txt");
                 num = progress.ReadLine();
                progress.Close();
                num1 = Int32.Parse(num);
            }
            if (times == 0)
            {
                for (int i = 0; i < num1; i++)
                {
                    words.ReadLine();
                }
            }
                    if (times == 0)
                        times = num1;
            

            word = words.ReadLine();
            Console.WriteLine(word+"");
            try
            {
                Tweet.PublishTweet("My anaconda don't want none unless you got " + word + " hun.");
            }
            catch (Exception)
            {
                Console.WriteLine("Error sending tweet!");
            }
            progresssave = new StreamWriter("Prog.txt");
            progresssave.Write(times);
            progresssave.Close();
            times++;
        }

        private static void readit()
        {
            
        }
    }
}
