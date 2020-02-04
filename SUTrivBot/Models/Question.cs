using System;
using System.Collections.Generic;
using System.Text;

namespace SUTrivBot.Models
{
    public class Question : IQuestion
    {
        // TODO: Should contain all necessary pieces to ask and then verify the answers to a trivia question

        private string _questionText;
        private string _answerText;

        public Question(string qText, string aText)
        {
            _questionText = qText;
            _answerText = aText;
        }

        public string GetQuestionText()
        {
            throw new NotImplementedException();
        }

        public bool VerifyAnswer(string answer)
        {
            throw new NotImplementedException();
        }
    }
}
