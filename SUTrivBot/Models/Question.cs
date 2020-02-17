using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SUTrivBot.Models
{
    public class Question
    {
        /// <summary>
        /// Text to be presented to the user for this Question.
        /// "What color is the sky?" for example
        /// </summary>
        [JsonProperty("questionText", Required = Required.Always)]
        public string QuestionText { get; set; }
        
        /// <summary>
        /// Text to be presented to the user as an answer for this Question.
        /// This field contains human readable text
        /// "Blue" for example
        /// </summary>
        [JsonProperty("answerText", Required = Required.Always)]
        public string AnswerText { get; set; }

        /// <summary>
        /// Set of strings representing all acceptable answers
        /// </summary>
        [JsonProperty("answers", Required = Required.Always)]
        public List<string> Answers { get; set; }

        /// <summary>
        /// If null, then all strings in Answers must be present in the user's response
        /// Otherwise, stores the count of required answers t 
        /// </summary>
        [JsonProperty("answersRequired", NullValueHandling = NullValueHandling.Ignore)]
        public int? AnswersRequired { get; set; }

        /// <summary>
        /// Set of strings representing all acceptable answers if the Question has a bonus portion
        /// </summary>
        [JsonProperty("bonusAnswers", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> BonusAnswers { get; set; }

        /// <summary>
        /// If true, then all strings in BonusAnswers must be present in the user's response
        /// </summary>
        [JsonProperty("bonusAll", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BonusAll { get; set; }

        /// <summary>
        /// Total base points this Question is worth
        /// </summary>
        [JsonProperty("points", Required = Required.Always)]
        public int Points { get; set; }

        /// <summary>
        /// Total Bonus points to be added to the base point value if this question has a bonus portion
        /// </summary>
        [JsonProperty("bonusPoints", NullValueHandling = NullValueHandling.Ignore)]
        public int? BonusPoints { get; set; }
        
        public int Id { get; set; }

        /// <summary>
        /// Verifies that the provided answer string is correct and if so, if it qualifies for the bonus answer
        /// if there is one.
        /// </summary>
        /// <param name="answer">Answer string from user</param>
        /// <returns></returns>
        public AnswerResponse VerifyAnswer(string answer)
        {
            var response = new AnswerResponse
            {
                AnswerStatus = AnswerStatus.Incorrect,
                Points = Points,
                BonusPoints = BonusPoints
            };

            bool AnswerContains(string item) => answer.Contains(item, StringComparison.InvariantCultureIgnoreCase);
            
            try
            {
                if (BonusPoints.HasValue && BonusPoints.Value != 0)
                {
                    response.BonusPoints = BonusPoints;
                    if (BonusAll ?? false)
                    {
                        if (BonusAnswers.All(AnswerContains)) response.AnswerStatus = AnswerStatus.BonusCorrect;
                        else if (BonusAnswers.Any(AnswerContains)) response.AnswerStatus = AnswerStatus.PartiallyCorrect;
                    }
                    else if (BonusAnswers.Any(AnswerContains)) response.AnswerStatus = AnswerStatus.BonusCorrect;

                    return response;
                }

                if (AnswersRequired.HasValue)
                {
                    if (Answers.All(AnswerContains)) response.AnswerStatus = AnswerStatus.NormalCorrect;
                    else if (Answers.Any(AnswerContains)) response.AnswerStatus = AnswerStatus.PartiallyCorrect;
                }
                else if (Answers.Count(AnswerContains) >= AnswersRequired) response.AnswerStatus = AnswerStatus.NormalCorrect;
                else if (Answers.Count(AnswerContains) > 0) response.AnswerStatus = AnswerStatus.PartiallyCorrect;

            }
            catch (Exception e) // Handling any errors is the responsibility of the consumer
            {
                response.AnswerStatus = AnswerStatus.Error;
                response.Exception = e;
            }

            return response;
        }

        // Overriding Equals for easier checking if a List of questions contains a given question or not
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is Question))
                return false;

            var otherQ = (Question) obj;

            return otherQ.Id == Id;
        }
    }
}
