using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;

namespace SUTrivBot.Models
{
    public class UserGameData
    {
        public Dictionary<string, string> QuestionsAnswered { get; } 
        public DiscordUser User { get; }
        public int Points { get; private set; }

        public UserGameData(DiscordUser user)
        {
            User = user;
            QuestionsAnswered = new Dictionary<string, string>();
            Points = 0;
        }

        public void AddAnswer(string questionText, string answerText, int points)
        {
            QuestionsAnswered.Add(questionText, answerText);
            Points += points;
        }

        public string GetGameData()
        {
            var strBuilder = new StringBuilder($"User: {User.Username} answered {QuestionsAnswered.Count} Questions \n");

            foreach (var (key, value) in QuestionsAnswered)
            {
                strBuilder.AppendLine($"\tQuestion: {key}");
                strBuilder.AppendLine($"\tAnswer: {value}");
                strBuilder.AppendLine();
            }

            strBuilder.AppendLine($"\tEarning a total of {Points} points!");

            return strBuilder.ToString();
        }
    }
}