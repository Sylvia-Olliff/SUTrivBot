using SUTrivBot.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace SUTrivBot.Repo
{
    public class TriviaStore : ITriviaStore
    {
        private Random _rnjesus;
        private ConcurrentDictionary<int, Question> _questions;
        private ILogger _logger;
        private bool _isLoaded;
        private TriviaStoreSettings _settings;

        public TriviaStore(ILogger logger, TriviaStoreSettings settings)
        {
            _rnjesus = new Random();
            _isLoaded = false;
            _questions = new ConcurrentDictionary<int, Question>();
            _logger = logger;
            _settings = settings;
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
                using (var file = new StreamReader(_settings.PathToFile))
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
                
                _logger.Debug($"{_questions.Count} Trivia Questions successfully loaded. ");
                _isLoaded = true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error Loading Trivia Questions Data!");
                throw;
            }
        }
    }
}
