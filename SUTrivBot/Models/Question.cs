using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SUTrivBot.Models
{
    public class Question : IQuestion
    {
        [JsonProperty("questionText", Required = Required.Always)]
        public string QuestionText { get; set; }

        [JsonProperty("answers", Required = Required.Always)]
        public List<string> Answers { get; set; }

        [JsonProperty("answerAll", Required = Required.Always)]
        public bool AnswerAll { get; set; }

        [JsonProperty("bonusAnswers", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> BonusAnswers { get; set; }

        [JsonProperty("bonusAll", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BonusAll { get; set; }

        [JsonProperty("points", Required = Required.Always)]
        public int Points { get; set; }

        [JsonProperty("bonusPoints", NullValueHandling = NullValueHandling.Ignore)]
        public int BonusPoints { get; set; }


        public string GetQuestionText()
        {
            return QuestionText;
        }

        public AnswerResponse VerifyAnswer(string answer)
        {
            if (BonusPoints != 0)
            {

            }
            else
            {

            }
            throw new NotImplementedException();
        }
    }
}
