using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using SUTrivBot.Lib;
using SUTrivBot.Models;
using System.Threading.Tasks;
using SUTrivBot.Repo;

namespace SUTrivBot
{
    public class Bot : IBot
    {
        private readonly DiscordClient _discord;
        private readonly CommandsNextModule _commands;
        private readonly InteractivityModule _interactivity;
        private readonly Config _config;

        public Bot()
        {
            var config = ConfigBuilder.Build();
            _config = config;

            _discord = new DiscordClient(config.DiscordConfiguration);

            _commands = _discord.UseCommandsNext(config.CommandsNextConfiguration);

            _commands.RegisterCommands<GameCommands>();
            _commands.RegisterCommands<TriviaMasterCommands>();
            _commands.RegisterCommands<FunCommands>();
            _commands.RegisterCommands<AdminCommands>();

            _interactivity = _discord.UseInteractivity(new InteractivityConfiguration());
        }

        public async Task StartAsync()
        {
            await GameMaster.InitGameMaster(new TriviaStore(_config.Logger, _config.TriviaStoreSettings), _discord, _config.Logger);
            await _discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
