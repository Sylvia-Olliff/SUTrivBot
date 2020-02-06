using System;
using System.Collections.Generic;
using System.Text;

namespace SUTrivBot.Models
{
    public class AnswerResponse
    {
        public AnswerStatus AnswerStatus { get; set; }
        public int Points { get; set; }
        public int? BonusPoints { get; set; }
        public Exception Exception { get; set; }
    }
}
