using System;
using System.Collections.Generic;
using System.Text;

namespace SUTrivBot.Models
{
    public class AnswerResponse
    {
        public bool AnswerCorrect { get; private set; }
        public bool BonusAnswerCorrect { get; private set; }
        public int Points { get; private set; }
        public int BonusPoints { get; private set; }
        public string ResponseText { get; private set; }

        public AnswerResponse(bool answerCorrect = false, bool bonusAnswerCorrect = false, int points = 1, int bonusPoints = 0, string responseText = "")
        {
            AnswerCorrect = answerCorrect;
            BonusAnswerCorrect = bonusAnswerCorrect;
            Points = points;
            BonusPoints = bonusPoints;
            ResponseText = responseText;
        }
    }
}
