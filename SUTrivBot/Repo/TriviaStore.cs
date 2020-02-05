using SUTrivBot.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SUTrivBot.Repo
{
    public class TriviaStore : ITriviaStore
    {
        private Random _rnjesus;
        private ConcurrentDictionary<int, Question> _questions;
        private bool _isLoaded;

        public TriviaStore()
        {
            _rnjesus = new Random();
            _isLoaded = false;
            _questions = new ConcurrentDictionary<int, Question>();
        }
        
        public Question GetRandomQuestion(List<int> excludedQuestions)
        {
            if (!_isLoaded)
                throw new ConstraintException("Trivia Store MUST be loaded before requesting a Question");

            int key;
            
            do key = _rnjesus.Next(1, _questions.Count);
            while (excludedQuestions.Contains(key));
            
            return _questions[key];
        }

        public async Task LoadQuestions()
        {
            try
            {
                // TODO: Replace string literal with config value for location and name of file
                using (var file = new StreamReader("./triviaQAData.json"))
                {
                    var dataSet = JsonConvert.DeserializeObject<TriviaDataSet>(await file.ReadToEndAsync());
                    
                    var count = 1;
                    foreach (var trivQ in dataSet.Questions)
                    {
                        trivQ.Id = count;
                        _questions.TryAdd(count, trivQ);
                        count++;
                    }
                }

                _isLoaded = true;
            }
            catch (Exception e)
            {
                // TODO: Add logging and error handling
                throw;
            }
        }
    }
}
