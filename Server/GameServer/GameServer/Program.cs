using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private static bool isRunning = false;
        private static string externalIp;
        private static string ipv4;

        static void Main(string[] args)
        {
            if (!IsConnectedToInternet())
            {
                Console.WriteLine("Unable to setup the server.\nPlease connect to an internet");
                return;
            }
            externalIp = GetExternalIP();
            ipv4 = GetIpv4();
            
            Console.Title = "Game Server";
            isRunning = true;
            
            int port = 7000;
            int maxPlayers = 2;

            // while (true)
            // {
            //     Console.WriteLine("How many players do you want in the lobby?");
            //     try
            //     {
            //         maxPlayers = Int32.Parse(Console.ReadLine()!);
            //
            //         if (maxPlayers < 2)
            //         {
            //             Console.WriteLine("Please put a value equal to or greater than 2!\n");
            //             continue;
            //         }
            //
            //         break;
            //     }
            //     catch
            //     {
            //         Console.WriteLine("Please enter a valid number!");
            //     }
            //     
            // }

            // while (true)
            // {
            //     Console.WriteLine("On what port do you want to host?");
            //     try
            //     {
            //         port = Int32.Parse(Console.ReadLine());
            //         if (port < 1024 || port > 49151)
            //         {
            //             Console.WriteLine("Invalid Port! \nPorts can only range from 1024 - 49151");
            //             continue;
            //         }
            //         
            //         break;
            //     }
            //     catch
            //     {
            //         Console.WriteLine("Please enter a valid port! \nPorts can only range from 1024 - 49151\n");
            //     }
            // }

            Console.WriteLine(
                "\n[i] IMPORTANT [i]" +
                $"\n--------------------------------------------------------------------\n" +
                $"Your ipv4 is {ipv4} \nSend this to people on the same wifi to play!\n\n" +
                $"Your external ip is {externalIp}Send this to people on different connections to play!\n\n" +
                $"MAKE SURE TO HAVE PORT FORWARDING SETUP FOR THE EXTERNAL IP TO WORK!\n" +
                $"The process is very simple and can be easily done by googling how to do so with your router model.\n" +
                $"--------------------------------------------------------------------\n");
            
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(2, port);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
        
        private static string GetExternalIP()  
        {
            return new WebClient().DownloadString("http://icanhazip.com");;
        }

        private static string GetIpv4()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[^1].ToString();
        }
        
        private static bool IsConnectedToInternet()  
        {  
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204")) 
                    return true; 
            }
            catch
            {
                return false;
            }
        }  
    }
}