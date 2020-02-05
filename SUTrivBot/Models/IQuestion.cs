using System;
using System.Collections.Generic;
using System.Text;

namespace SUTrivBot.Models
{
    public interface IQuestion
    {
        public string GetQuestionText();
        public AnswerResponse VerifyAnswer(string answer);
    }
}
