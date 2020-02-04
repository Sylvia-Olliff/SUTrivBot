using System;
using System.Threading.Tasks;

namespace SUTrivBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IBot discBot = new Bot(Environment.GetEnvironmentVariable("BOT_TOKEN"));

            await discBot.StartAsync();
        }
    }
}
