using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;



namespace MelonBot.Bots
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
