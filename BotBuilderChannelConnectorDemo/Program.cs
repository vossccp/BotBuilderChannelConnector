using System;
using Microsoft.Owin.Hosting;

namespace Bot.Builder.ChannelConnector.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            const string baseAddress = "http://localhost:9000/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Listening to {baseAddress}");
                Console.WriteLine("Press return");
                Console.ReadLine();
            }
        }
    }
}
