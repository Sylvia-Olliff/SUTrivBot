using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SUTrivBot.Models
{
    public class Question
    {
        [JsonProperty("questionText", Required = Required.Always)]
        public string QuestionText { get; set; }

        [JsonProperty("answers", Required = Required.Always)]
        public List<string> Answers { get; set; }

        [JsonProperty("answerAll", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AnswerAll { get; set; }

        [JsonProperty("bonusAnswers", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> BonusAnswers { get; set; }

        [JsonProperty("bonusAll", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BonusAll { get; set; }

        [JsonProperty("points", Required = Required.Always)]
        public int Points { get; set; }

        [JsonProperty("bonusPoints", NullValueHandling = NullValueHandling.Ignore)]
        public int? BonusPoints { get; set; }
        
        public int Id { get; set; }

        public AnswerResponse VerifyAnswer(string answer)
        {
            var response = new AnswerResponse
            {
                AnswerStatus = AnswerStatus.Incorrect,
                Points = Points,
                BonusPoints = BonusPoints
            };

            try
            {
                if (BonusPoints.HasValue && BonusPoints.Value != 0)
                {
                    response.BonusPoints = BonusPoints;
                    if (BonusAll ?? false)
                    {
                        if (BonusAnswers.All(item =>
                            answer.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                            response.AnswerStatus = AnswerStatus.BonusCorrect;
                        else if (BonusAnswers.Any(item =>
                            answer.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                            response.AnswerStatus = AnswerStatus.PartiallyCorrect;
                    }
                    else if (BonusAnswers.Any(item =>
                        answer.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                        response.AnswerStatus = AnswerStatus.BonusCorrect;

                    return response;
                }

                if (AnswerAll ?? false)
                {
                    if (Answers.All(item =>
                        answer.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                        response.AnswerStatus = AnswerStatus.NormalCorrect;
                    else if (Answers.Any(item =>
                        answer.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                        response.AnswerStatus = AnswerStatus.PartiallyCorrect;
                }
                else if (Answers.Any(item =>
                    answer.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                    response.AnswerStatus = AnswerStatus.PartiallyCorrect;


            }
            catch (Exception e)
            {
                // TODO: Add logging and other error handling
                response.AnswerStatus = AnswerStatus.Error;
            }

            return response;
        }
    }
}
