using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;

namespace SUTrivBot.Models
{
    public class UserGameData
    {
        public Dictionary<int, AnswerData> QuestionsAnswered { get; } 
        public DiscordUser User { get; }
        public int Points { get; private set; }

        public UserGameData(DiscordUser user)
        {
            User = user;
            QuestionsAnswered = new Dictionary<int, AnswerData>();
            Points = 0;
        }

        public void AddAnswer(Question question, string answerText, int points)
        {
            QuestionsAnswered.Add(question.Id, new AnswerData
            {
                Question = question,
                Answer = answerText,
                PointsEarned = points
            });
            Points += points;
        }

        public string GetGameData()
        {
            var strBuilder = new StringBuilder($"User: {User.Username} answered {QuestionsAnswered.Count} Questions \n");

            foreach (var (key, value) in QuestionsAnswered)
            {
                strBuilder.AppendLine($"\t# {key}");
                strBuilder.AppendLine($"\tQuestion: {value.Question.QuestionText}");
                strBuilder.AppendLine($"\tAnswer: {value.Answer}");
                strBuilder.AppendLine($"\tEarning {value.PointsEarned}");
                strBuilder.AppendLine();
            }

            strBuilder.AppendLine($"\tEarning a total of {Points} points!");

            return strBuilder.ToString();
        }
    }
}