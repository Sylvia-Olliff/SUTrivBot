using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<DiscordUser, UserGameData> _players;
        private readonly List<Question> _askedQuestions;
        private int _roundCount;
        

        public GameState(GameId gameId, ILogger logger)
        {
            _channel = gameId.Channel;
            _guild = gameId.Guild;
            _triviaMaster = gameId.User;
            _logger = logger;
            _players = new ConcurrentDictionary<DiscordUser, UserGameData>();
            _askedQuestions = new List<Question>();
            _roundCount = 0;
        }

        public string GetGameName()
        {
            return $"Game in Channel {_channel.Name} in Guild {_guild.Name}";
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
                    _players[msg.Author].QuestionsAnswered.ContainsKey(question.QuestionText))
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
                            player.AddAnswer(question.QuestionText, msg.Content, (int)((double)question.Points / 2));
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question.QuestionText, msg.Content, (int)((double)question.Points / 2));
                            _players.TryAdd(msg.Author, userData);
                        }

                        break;
                    case AnswerStatus.Error:
                        _logger.Error(result.Exception, $"Error parsing command for question: {question.QuestionText}");
                        
                        break;
                    case AnswerStatus.NormalCorrect:
                        if (_players.ContainsKey(msg.Author))
                        {
                            var player = _players[msg.Author];
                            player.AddAnswer(question.QuestionText, msg.Content, question.Points);
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question.QuestionText, msg.Content, question.Points);
                            _players.TryAdd(msg.Author, userData);
                        }

                        response = true;
                        break;
                    case AnswerStatus.BonusCorrect:
                        if (_players.ContainsKey(msg.Author))
                        {
                            var player = _players[msg.Author];
                            player.AddAnswer(question.QuestionText, msg.Content, question.BonusPoints.Value);
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question.QuestionText, msg.Content, question.BonusPoints.Value);
                            _players.TryAdd(msg.Author, userData);
                        }

                        response = true;
                        break;
                    case AnswerStatus.Incorrect:
                        if (_players.ContainsKey(msg.Author))
                        {
                            var player = _players[msg.Author];
                            player.AddAnswer(question.QuestionText, msg.Content, 0);
                        }
                        else
                        {
                            var userData = new UserGameData(msg.Author);
                            userData.AddAnswer(question.QuestionText, msg.Content, 0);
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
            var strBuilder = new StringBuilder("### *Game Results* ###\n");
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
    }
}