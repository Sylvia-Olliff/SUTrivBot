using SUTrivBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SUTrivBot.Repo
{
    public interface ITriviaStore
    {
        public Task LoadQuestions();
        public IQuestion GetRandomQuestion();
    }
}
