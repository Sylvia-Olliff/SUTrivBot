using SUTrivBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SUTrivBot.Repo
{
    public class TriviaStore : ITriviaStore
    {
        // TODO: Load all questions from storage (probably a file?)

        public IQuestion GetRandomQuestion()
        {
            throw new NotImplementedException();
        }

        public Task LoadQuestions()
        {
            throw new NotImplementedException();
        }
    }
}
