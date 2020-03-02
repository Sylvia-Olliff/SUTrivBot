using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using NLog;
using SUTrivBot.Models;
using SUTrivBot.Repo;

namespace SUTrivBot.Lib
{
    public static class GameMaster
    {
        private static ILogger _logger;
        private static ConcurrentDictionary<GameId, IGameState> _games;
        private static ITriviaStore _triviaStore;
        private static DiscordClient _client;

        public static async Task InitGameMaster(ITriviaStore triviaStore, DiscordClient client, ILogger logger)
        {
            _triviaStore = triviaStore;
            _logger = logger;
            _games = new ConcurrentDictionary<GameId, IGameState>();
            _client = client;
            
            await _triviaStore.LoadQuestions();
        }

        public static async Task<bool> NewGame(GameId gameId)
        {
            if (_games.ContainsKey(gameId))
                return false;

            await using var db = new SettingsContext();
            var guildSettings = db.Guilds.FindAsync("GuildId", gameId.Guild.Name).Result;

            if (guildSettings == null)
            {
                guildSettings = new GuildSettings
                {
                    GuildId = gameId.Guild.Name,
                    Disabled = false,
                    RestrictTrivMaster = true
                };
                await db.Guilds.AddAsync(guildSettings);
            }
            
            _games.TryAdd(gameId, new GameState(gameId, _client, guildSettings, _logger));
            return true;
        }

        public static IGameState GetGameById(GameId gameId)
        {
            return _games.ContainsKey(gameId) ? _games[gameId] : null;
        }

        public static IGameState GetGameByName(string name)
        {
            return _games.First(g => g.Key.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase)).Value;
        }

        public static IGameState GetGameByTriviaMaster(DiscordUser user)
        {
            return _games.First(g => g.Key.User.Equals(user)).Value;
        }

        public static List<IGameState> GetAllGames()
        {
            return _games.Values.ToList();
        }

        public static async Task ResolveGame(CommandContext ctx, GameId gameId)
        {
            if (_games.ContainsKey(gameId))
            {
                var game = _games[gameId];
                await game.GetResults(ctx);
                _games.TryRemove(gameId, out _);
            }
        }

        public static async Task GetGameStatus(CommandContext ctx, GameId gameId)
        {
            if (_games.ContainsKey(gameId))
                await _games[gameId].GetStatus(ctx);
        }

        public static Question GetQuestion(List<int> excludedQuestions = null)
        {
            return _triviaStore.GetRandomQuestion(excludedQuestions);
        }

        public static Question GetQuestionById(int id)
        {
            return _triviaStore.GetQuestionById(id);
        }
    }
}