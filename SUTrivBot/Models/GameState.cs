using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using NLog;
using SUTrivBot.Lib;

namespace SUTrivBot.Models
{
    public class GameState : IGameState
    {
        private readonly DiscordChannel _channel;
        private readonly DiscordGuild _guild;
        private readonly DiscordUser _triviaMaster;
        private readonly DiscordClient _client;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<DiscordUser, UserGameData> _players;
        private readonly List<Question> _askedQuestions;
        private readonly GuildSettings _guildSettings;
        
        private DiscordDmChannel _triviaMasterDmChannel;
        private int _roundCount;
        

        public GameState(GameId gameId, DiscordClient client, GuildSettings guildSettings, ILogger logger)
        {
            _channel = gameId.Channel;
            _guild = gameId.Guild;
            _triviaMaster = gameId.User;
            _logger = logger;
            _players = new ConcurrentDictionary<DiscordUser, UserGameData>();
            _askedQuestions = new List<Question>();
            _roundCount = 0;
            _client = client;
            _guildSettings = guildSettings;
            
            EstablishTriviaMasterDm().Wait();
        }

        public string GetGameName()
        {
            return $"Game in Channel {_channel.Name} in Guild {_guild.Name}";
        }

        public DiscordUser GetTriviaMaster()
        {
            return _triviaMaster;
        }

        public async Task AskQuestion(CommandContext ctx)
        {
            var question = GameMaster.GetQuestion(_askedQuestions.Select(q => q.Id).ToList());
            await ctx.RespondAsync($"Question: {question.QuestionText} [Time limit: 1 min]");
            
            var interactivity = ctx.Client.GetInteractivityModule();
            
            var answer = await interactivity.WaitForMessageAsync(msg =>
            {
                // If this user has already answered this question, do not accept any new answers.
                if (_players.ContainsKey(msg.Author) &&
                    _players[msg.Author].QuestionsAnswered.ContainsKey(question.Id))
                {
                    msg.DeleteAsync();
                    return false;
                }
                
                var result = question.VerifyAnswer(msg.Content);
                var response = false;
                switch (result.AnswerStatus)
                {
                    case AnswerStatus.PartiallyCorrect:
                        if (_players.ContainsKey(msg.Author))
                        {
                            var player = _players[msg.Author];
                            player.AddAnswer(question, msg.Content, (int)((double)question.Points / 2));
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question, msg.Content, (int)((double)question.Points / 2));
                            _players.TryAdd(msg.Author, userData);
                        }

                        break;
                    case AnswerStatus.Error:
                        _logger.Error(result.Exception, $"Error parsing command for question: {question.QuestionText}");
                        _triviaMasterDmChannel.SendMessageAsync(
                            $"Error occured processing an answer! \nAnswer: {msg.Content}\nError Message: {result.Exception.Message}\n");
                        break;
                    case AnswerStatus.NormalCorrect:
                        if (_players.ContainsKey(msg.Author))
                        {
                            var player = _players[msg.Author];
                            player.AddAnswer(question, msg.Content, question.Points);
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question, msg.Content, question.Points);
                            _players.TryAdd(msg.Author, userData);
                        }

                        response = true;
                        break;
                    case AnswerStatus.BonusCorrect:
                        if (_players.ContainsKey(msg.Author))
                        {
                            var player = _players[msg.Author];
                            player.AddAnswer(question, msg.Content, question.BonusPoints.Value);
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question, msg.Content, question.BonusPoints.Value);
                            _players.TryAdd(msg.Author, userData);
                        }

                        response = true;
                        break;
                    case AnswerStatus.Incorrect:
                        if (_players.ContainsKey(msg.Author))
                        {
                            var player = _players[msg.Author];
                            player.AddAnswer(question, msg.Content, 0);
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question, msg.Content, 0);
                            _players.TryAdd(msg.Author, userData);
                        }
                        break;
                }

                msg.DeleteAsync();
                return response;
            }, TimeSpan.FromMinutes(1));

            if (answer != null)
            {
                await ctx.RespondAsync($"User: {answer.User.Mention} answered correctly! [Round Over]");
            }
            else
            {
                await ctx.RespondAsync("Timeout reached! [Round Over]");
            }

            _roundCount++;
            
            _askedQuestions.Add(question);
        }

        public async Task GetResults(CommandContext ctx)
        {
            var strBuilder = new StringBuilder("### _Game Results_ ###\n");
            strBuilder.AppendLine($"Rounds played: {_roundCount}");
            strBuilder.AppendLine();

            var highestPlayer = new UserGameData(null);
            
            
            foreach (var (user, gameData) in _players)
            {
                strBuilder.AppendLine($"{user.Username} results:");
                if (gameData.Points > highestPlayer.Points)
                    highestPlayer = gameData;
                strBuilder.Append(gameData.GetGameData());
                strBuilder.AppendLine();
            }

            strBuilder.AppendLine($"Player: {highestPlayer.User.Mention} Wins with {highestPlayer.Points} points!");
            
            await ctx.RespondAsync(strBuilder.ToString());
        }

        public async Task GetStatus(CommandContext ctx)
        {
            var strBuilder = new StringBuilder("### _Game Status_ ###\n");
            strBuilder.AppendLine($"Rounds played: {_roundCount}");
            strBuilder.AppendLine();

            foreach (var (user, gameData) in _players)
            {
                strBuilder.AppendLine($"{user.Username} results:");
                strBuilder.Append(gameData.GetGameData());
                strBuilder.AppendLine();
            }

            await ctx.RespondAsync(strBuilder.ToString());
        }

        public async Task GetPoints(CommandContext ctx)
        {
            var strBuilder = new StringBuilder("Points by player\n");

            var pointList = new List<Tuple<string, int>>();
            foreach (var (user, gameData) in _players)
            {
                pointList.Add(new Tuple<string, int>(user.Username, gameData.Points));
            }
            pointList.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            foreach (var (username, points) in pointList)
            {
                strBuilder.AppendLine($"{username} with {points}");
            }
            
            await ctx.RespondAsync(strBuilder.ToString());
        }

        private async Task EstablishTriviaMasterDm()
        {
            _triviaMasterDmChannel = await _client.CreateDmAsync(_triviaMaster);
            await _triviaMasterDmChannel.SendMessageAsync(
                $"You have been established as the Trivia Master for a Trivia {GetGameName()}");
        }
    }
}